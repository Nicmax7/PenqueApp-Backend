using Microsoft.EntityFrameworkCore;

namespace BackEnd.Models.Clases
{
    public enum Tipo_Historial
    {
        Gano,
        Perdio,
        Empato
    }
  
    public class Historial
    {
        public int Id { get; set; } 
        public Tipo_Historial tipo_Historial { get; set; }
    }
}
