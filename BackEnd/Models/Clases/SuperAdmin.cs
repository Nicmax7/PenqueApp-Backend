namespace BackEnd.Models.Clases
{
    public class SuperAdmin
    {
        public int Id { get; set; }
        public string email { get; set; }

        public string password { get; set; }

        public string nombre { get; set; }

        public float economia { get; set; }

        public Tipo_Rol Tipo_Rol { get; set; }
    }
}
