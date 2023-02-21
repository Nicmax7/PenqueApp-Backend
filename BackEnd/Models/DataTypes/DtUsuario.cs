using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtUsuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string email { get; set; }

        public float? billetera { get; set; }

        public Tipo_Rol tipo_rol { get; set; }
    }
}
