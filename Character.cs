class Character
{
    private int _age;
    public string Name { get; set; }
    public int Age 
    { 
        get
        {
            return _age;
        }
        set 
        {
            if(value>0)
            {
                _age = value;
            }
        } 
    }
    public string ClassName { get; set; }
    public Character(string characterName, int characterAge, string characterClassName)
    {
        Name = characterName;
        Age = characterAge;
        ClassName = characterClassName;
    }
}