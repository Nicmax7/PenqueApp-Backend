using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtChat
    {
        public int Id { get; set; }

        public string usuario { get; set; }

        public string empresa { get; set; }

        public int idusuario { get; set; }
        public int idempresa { get; set; }

    }
}
