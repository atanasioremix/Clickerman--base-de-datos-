using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControler : MonoBehaviour
{

	public GameObject ClickerUI;
	private User user;
	private DatabaseHandler DB;

	public Text Marcador;
	public Text Mensaje;

	public Button ClikerButton;
	public Button RegisterButton;
	public Button LoginButton;
	public Button SaveButton;

	public InputField NicknameField;
	public InputField PasswordField;

	void Start()
	{
		DB = GetComponent<DatabaseHandler>();
		ClikerButton.onClick.AddListener(AddPoints);
		RegisterButton.onClick.AddListener(Register);
		LoginButton.onClick.AddListener(Login);
		SaveButton.onClick.AddListener(Save);
	}

	void AddPoints()
	{
		user.score++;
		Marcador.text = user.score.ToString();
		
	}

	void Register()
	{
		if (NicknameField.text.Equals("") || PasswordField.text.Equals(""))
		{
			Mensaje.text = "Introduce un usuario y contraseña";
			return;
		}
		bool resultado = DB.Registrar(NicknameField.text, PasswordField.text);
		if(resultado)
        {
			Mensaje.text = "Usuario registrado, inicie sesión";
        }
		else
        {
			Mensaje.text = "Usuario ya registrado";
		}

	}

	void Login()
    {
		user = DB.IniciarSesion(NicknameField.text, PasswordField.text);
		if(user != null)
        {
			ClickerUI.SetActive(true);
			Mensaje.text = "Has iniciado sesión como " + user.nickname;
			Marcador.text = user.score.ToString();
		}
		else
        {
			Mensaje.text = "Usuario o contraseña incorrecto";
        }

    }

	void Save()
    {

		DB.GuardarDatosDB(user);
		DB.GuardarJSON(user);
		Mensaje.text = "Guardado con éxito";
	}
	
}
