using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Librerías que necesitamos
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using Newtonsoft.Json;

public class DatabaseHandler : MonoBehaviour
{
    //Variable para controlar la ruta de la base de datos, constructor de la ruta, y el nombre de la base de datos
    string rutaDB;
    string strConexion;
    string DBFileName = "UserData.db";

    //Variable para trabajar con las conexiones
    IDbConnection dbConnection;

    //Para poder ejecutar comandos
    IDbCommand dbCommand;

    //Variable para leer
    IDataReader reader;

    // Start is called before the first frame update
    void Start()
    {
        SetupDB();

    }

    //Método para abrir la DB
    void AbrirDB()
    {
        // Crear y abrir la conexión
        // Comprobar en que plataforma estamos
        // Si es el Editor de Unity mantenemos la ruta
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            rutaDB = Application.dataPath + "/StreamingAssets/" + DBFileName;
        }
        //Si estamos en PC
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            rutaDB = Application.dataPath + "/StreamingAssets/" + DBFileName;
        }
        //Si es Android
        else if (Application.platform == RuntimePlatform.Android)
        {
            rutaDB = Application.persistentDataPath + "/" + DBFileName;
            // Comprobar si el archivo se encuentra almecenado en persistant data
            if (!File.Exists(rutaDB))
            {
                // Almaceno el archivo en load db
                WWW loadDB = new WWW("jar;file://" + Application.dataPath + "!/assets/" + DBFileName);
                while (!loadDB.isDone)
                {
                }

                // Copio el archivo a persistant data
                File.WriteAllBytes(rutaDB, loadDB.bytes);
            }
        }

        strConexion = "URI=file:" + rutaDB;
        dbConnection = new SqliteConnection(strConexion);
        dbConnection.Open();
    }

    void SetupDB()
    {
        //Abrimos la DB
        AbrirDB();
        // Crear la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = @"CREATE TABLE IF NOT EXISTS User(  
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            nickname VARCHAR(255) UNIQUE NOT NULL,
            password VARCHAR(255) NOT NULL,
            score INTEGER NOT NULL DEFAULT '0'
            )";
        dbCommand.CommandText = sqlQuery;
        dbCommand.ExecuteScalar();
        //Cerramos la DB
        CerrarDB();
    }

    public User IniciarSesion(string nickname, string password)
    {
        //Abrimos la DB
        AbrirDB();
        // Crear la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = string.Format("SELECT * FROM User WHERE nickname = \"{0}\" AND password = \"{1}\"", nickname,
            password);
        dbCommand.CommandText = sqlQuery;

        // Leer la base de datos
        reader = dbCommand.ExecuteReader();
        // Si no se ha encontrado un usuario con el nick y contraseña dados devolver null
        if (!reader.Read()) return null;
        // Creamos el objeto usuario si se ha recuperado de la base de datos
        var user = new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3));
        reader.Close();
        reader = null;
        //Cerramos la DB
        CerrarDB();
        return user;
    }

    public bool Registrar(string nickname, string password)
    {
        //Abrimos la DB
        AbrirDB();
        // Crear la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = String.Format("INSERT INTO User(nickname, password) values(\"{0}\",\"{1}\")", nickname,
            password);
        dbCommand.CommandText = sqlQuery;
        try
        {
            dbCommand.ExecuteScalar();
        }
        catch (Exception e)
        {
            return false;
        }

        //Cerramos la DB
        CerrarDB();

        return true;
    }

    public bool GuardarDatosDB(User user)
    {
        //Abrimos la DB
        AbrirDB();
        // Crear la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = String.Format("UPDATE User SET score = \"{0}\" WHERE id = \"{1}\"",
            user.score,
            user.id);
        
        dbCommand.CommandText = sqlQuery;
        try
        {
            dbCommand.ExecuteScalar();
        }
        catch (Exception e)
        {
            return false;
        }

        //Cerramos la DB
        CerrarDB();

        return true;
    }

    public void GuardarJSON(User user)
    {
        string json = JsonUtility.ToJson(user, true);

        StreamWriter writer = new StreamWriter(Application.dataPath + "/JsonGuardado/" + user.nickname + ".json", false);
        writer.Write(json);
        writer.Close();
    }

    //Método para cerrar la DB
    void CerrarDB()
    {
        // Cerrar las conexiones
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }
}