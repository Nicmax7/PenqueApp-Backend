using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtEmpresa
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public float? Billetera { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public Tipo_Rol tipo_rol { get; set; }
    }
}
