namespace BackEnd.Models.Clases
{

    public class Participante
    {
        public int Id { get; set; }
        public string nombre { get; set; }
        public List<Competencia> competencias { get; set; }
        public Tipo_Area Area { get; set; }
        public string pais { get; set; }
        public Participante()
        {
            competencias = new List<Competencia>();
        }
    }
}
