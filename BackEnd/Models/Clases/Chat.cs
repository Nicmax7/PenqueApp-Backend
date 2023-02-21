namespace BackEnd.Models.Clases
{
    public class Chat
    {

        public int Id { get; set; }

        public List<Mensaje> mensajes { get; set; }

        public Usuario usuario { get; set; }

        public Empresa empresa { get; set; }

        public Chat()
        {
            mensajes = new List<Mensaje>();
        }
    }
}
