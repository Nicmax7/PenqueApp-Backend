using System;
using System.Runtime.InteropServices;


namespace BackEnd.Models.Clases{

	public class Liga_Equipo{

        public int id { get; set; }
        public string nombreLiga { get; set; }
		public List<Partido> partidos { get; set; }
		public List<Penca> pencas { get; set; }
		public int topePartidos { get; set; }
        public bool activa { get; set; }

        public Tipo_Deporte tipoDeporte { get; set; }

        public Liga_Equipo() { 
			partidos = new List<Partido>();
		}
        public bool actualizarEstado()
        {
            this.activa = false;
            
            foreach (var partido in partidos)
            {
                if(topePartidos != 0)
                {
                    activa = true;
                    break;
                }
                if (partido.resultado == Tipo_Resultado.Indefinido)
                {  
                    this.activa = true;
                    break;
                }
            }
            return this.activa;
        }
    }
}