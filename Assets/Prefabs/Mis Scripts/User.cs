using System;

[Serializable]
public class User
{
    public int id;
    public string nickname;
    public string password;
    public int score;

    public User(int id, string nickname, string password, int score)
    {
        this.id = id;
        this.nickname = nickname;
        this.password = password;
        this.score = score;
    }
}
