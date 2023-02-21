namespace BackEnd.Models.DataTypes
{
    public class DtMensajeForo
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdPenca { get; set; }
        public string Comentario { get; set; }
    }
}
