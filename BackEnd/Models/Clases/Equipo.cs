using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace BackEnd.Models.Clases{
    public enum Division
    {
        Primera,
        Segunda,
        Tercera
    }
    public class Equipo{

        public int id { get; set; }

        public string nombreEquipo { get; set; }

        public List<Historial> historiales { get; set; }

        public List<Partido> partidos { get; set; }

        public Tipo_Deporte deporte { get; set; }

        public string pais { get; set; }

        public Division division { get; set; }

        public void agregarHistorial(Tipo_Historial tipo)
        {
            Historial historial = new Historial();
            historial.tipo_Historial = tipo;
            if (this.historiales == null)
            {
                this.historiales = new List<Historial>();
            }
            this.historiales.Add(historial);
        }
    }
}
