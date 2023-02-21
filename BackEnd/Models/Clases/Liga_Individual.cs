using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Models.Clases
{
    public class Liga_Individual
    {
        public int Id { get; set; }
        public string Nombre { get; set; } 
        public List<Competencia> competencias { get; set; }
        public List<Penca> pencas { get; set; }
        public int topeCompetencias { get; set; }
        public bool activa { get; set; }

        public Tipo_Area tipoArea { get; set; }
        public Liga_Individual()
        {
            competencias = new List<Competencia>();
            pencas = new List<Penca>();
        }

        public static implicit operator Liga_Individual(Liga_Equipo v)
        {
            throw new NotImplementedException();
        }
        public bool actualizarEstado(List<Competencia> listCompetencias)
        {
            this.activa = false;
            foreach (var competencia in listCompetencias)
            {
                if (competencia.posiciones.Count() == 0)
                {
                    activa = true;
                    break;
                }
                
                if(listCompetencias.Count() < this.topeCompetencias || listCompetencias == null)
                {
                    activa = true;
                    break;
                    
                }
            }
        return this.activa;
        }
    }
}
