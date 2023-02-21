using BackEnd.Data;
using BackEnd.Models.Clases;
using BackEnd.Models.DataTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using System.Net.Mail;
using System.Runtime.Intrinsics.X86;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartidoController : ControllerBase
    {
        private readonly EntidadesDbContext _context;
        public PartidoController(EntidadesDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Partido>> Get()
        {
            return await _context.Partidos.ToListAsync();
        }

        [HttpGet("getPartidosSinUsar")]
        [ProducesResponseType(typeof(Partido), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> getPartidosSinUsar(Tipo_Deporte tipo)
        {
            var partidos = _context.Partidos.ToList();
            List<DtPartido> dtpartidos = new List<DtPartido>();
            
            foreach (var aux in partidos)
            {
                if (!aux.enUso && aux.resultado == Tipo_Resultado.Indefinido && aux.deporte == tipo)
                {
                    var local = await _context.Equipos.FindAsync(aux.IdLocal);
                    if (local == null) return BadRequest("No existe el equipo");
                    var visitante = await _context.Equipos.FindAsync(aux.IdVisitante);
                    if (visitante == null) return BadRequest("No existe el equipo");

                    DtPartido dtpartido = new DtPartido();
                    dtpartido.fecha = aux.fechaPartido;
                    dtpartido.resultado = aux.resultado;
                    dtpartido.id = aux.id;
                    dtpartido.local = local.nombreEquipo;
                    dtpartido.visitante = visitante.nombreEquipo;
                    dtpartidos.Add(dtpartido);
                }
            }
            return dtpartidos == null ? NotFound() : Ok(dtpartidos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Partido), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id)
        {
            var partidos = _context.Partidos.ToList();
            DtPartido dtpartido = new DtPartido();
            foreach (var aux in partidos)
            {
                if(aux.id == id)
                {
                    var local = await _context.Equipos.FindAsync(aux.IdLocal);
                    if (local == null) return BadRequest("No existe el equipo");
                    var visitante = await _context.Equipos.FindAsync(aux.IdVisitante);
                    if (visitante == null) return BadRequest("No existe el equipo");

                    dtpartido.fecha = aux.fechaPartido;
                    dtpartido.resultado = aux.resultado;
                    dtpartido.local = local.nombreEquipo;
                    dtpartido.Idlocal = local.id;
                    dtpartido.visitante = visitante.nombreEquipo;
                    dtpartido.Idvisitante = visitante.id;
                    dtpartido.id = aux.id;
                    dtpartido.deporte = aux.deporte;
                }
            }

            

            return partidos == null ? NotFound() : Ok(dtpartido);
        }

        [HttpPost("altaPartido")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> altaEquipo(DtPartido dtpartido)
        {
            Partido partido = new Partido();
            

            Equipo? local = await _context.Equipos.FindAsync(dtpartido.Idlocal);
            Equipo? visitante = await _context.Equipos.FindAsync(dtpartido.Idvisitante);

            if (local == null || visitante == null) return BadRequest("Uno de los equipos ingresados es NULL");
            if (local.id == visitante.id) return BadRequest("Debes elegir equipos distintos");


            Console.WriteLine("Local: " + local.id);
            Console.WriteLine("Visitante: " + visitante.id);
            partido.deporte = dtpartido.deporte;
            partido.fechaPartido = dtpartido.fecha;
            partido.IdLocal = local.id;
            partido.IdVisitante = visitante.id;
            partido.enUso = false;
            partido.resultado = Tipo_Resultado.Indefinido;

            if(local.partidos == null)
            {
                local.partidos = new List<Partido>();
            }
            local.partidos.Add(partido);
            if (visitante.partidos == null)
            {
                visitante.partidos = new List<Partido>();
            }
            visitante.partidos.Add(partido);

            await _context.Partidos.AddAsync(partido);
            _context.Entry(visitante).State = EntityState.Modified;
            _context.Entry(local).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            dtpartido.id = partido.id;

            return CreatedAtAction(nameof(GetById), new { id = dtpartido.id }, dtpartido);
        }

        [HttpGet("EquiposGet/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEquiposById(int id)
        {
            var partido =  _context.Partidos;
            List<DtEquipo> equipoList = new List<DtEquipo>();
            foreach(var item in partido)
            {
                if(item.id == id)
                {
                    var local = await _context.Equipos.FindAsync(item.IdLocal);
                    if (local == null) return BadRequest("No existe el equipo");
                    var visitante = await _context.Equipos.FindAsync(item.IdVisitante);
                    if (visitante == null) return BadRequest("No existe el equipo");

                    DtEquipo equipo = new DtEquipo();
                    equipo.Id = local.id;
                    equipo.Name = local.nombreEquipo;
                    equipo.Deporte = local.deporte;
                    equipo.Division = local.division;
                    equipo.Pais = local.pais;
                    equipoList.Add(equipo);

                    equipo.Id = visitante.id;
                    equipo.Name = visitante.nombreEquipo;
                    equipo.Deporte = visitante.deporte;
                    equipo.Division = visitante.division;
                    equipo.Pais = visitante.pais;
                    equipoList.Add(equipo);

                    return Ok(equipoList);
                }
            }
          
            return  NotFound();
        }

        [HttpPut("actualizarResultado/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> actualizarResultado(int id)
        {

            var partido = await _context.Partidos.FindAsync(id);
            if (partido == null) return BadRequest("No existe el partido");

            var local = await _context.Equipos.FindAsync(partido.IdLocal);
            if (local == null) return BadRequest("No existe el equipo");
            var visitante = await _context.Equipos.FindAsync(partido.IdVisitante);
            if (visitante == null) return BadRequest("No existe el equipo");

            var random = new Random();
            if(partido.deporte == Tipo_Deporte.Futbol) {
                partido.resultado = (Tipo_Resultado)random.Next(Enum.GetNames(typeof(Tipo_Resultado)).Length - 1);
            }
            else
            {
               partido.resultado = (Tipo_Resultado)random.Next(Enum.GetNames(typeof(Tipo_Resultado)).Length - 2);
            }
            

            _context.Entry(partido).State = EntityState.Modified;

            var partido2 = _context.Partidos;

            Partido play = new Partido();
            foreach (var item in partido2)
            {
                if (item.id == id)
                {
                    play = item;
                    break;
                }
            }
            if(play.resultado == Tipo_Resultado.Empate) {
                local.agregarHistorial(Tipo_Historial.Empato);
                visitante.agregarHistorial(Tipo_Historial.Empato);
            }
            if(play.resultado == Tipo_Resultado.Local)
            {
                local.agregarHistorial(Tipo_Historial.Perdio);
                visitante.agregarHistorial(Tipo_Historial.Gano);
            }
            if (play.resultado == Tipo_Resultado.Visitante)
            {
                local.agregarHistorial(Tipo_Historial.Gano);
                visitante.agregarHistorial(Tipo_Historial.Perdio);
            }

            _context.Entry(play).State = EntityState.Modified;
            

            //SumarPuntos();
            var partido3 = _context.Partidos.Include(p => p.predicciones);
            foreach (var item in partido3)
            {
                if (item.id == id)
                {
                    play = item;
                    break;
                }
            }
 
            foreach (var item in play.predicciones)
            {
                if(item.tipo_Resultado == partido.resultado)
                {
                    var puntos = await _context.Puntuaciones.FindAsync(item.idPuntuacionUsuario);
                    if(puntos != null)
                    {
                        puntos.puntos++;
                        _context.Entry(puntos).State = EntityState.Modified;
                    }
                }
            }

            //enviar correo
            //var partido3 = _context.Partidos.Include(p => p.predicciones);
            List<Prediccion> prediccions = new List<Prediccion>();
            var prediccionesUsuarios = _context.Usuario.Include(u => u.predicciones);
            foreach (var partidoAux in partido3)
            {
                if(partidoAux.id == id)
                {
                    prediccions = partidoAux.predicciones;
                }
            }

            foreach (var usuarioAux in prediccionesUsuarios)
            {
                foreach (var prediccionUsu in usuarioAux.predicciones)
                {
                    foreach(var prediccionPart in prediccions)
                    {
                        if(prediccionUsu.Id == prediccionPart.Id)
                        {
                            string texto = "Saludos " + usuarioAux.nombre + ", le avisamos que un partido en el que ha apostado finalizó.";
                            MailMessage mensaje = new MailMessage("penqueapp@gmail.com", usuarioAux.email, "[PenqueApp] Finalizó el Partido esperado", texto);
                            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                            smtpClient.EnableSsl = true;
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Host = "smtp.gmail.com";
                            smtpClient.Port = 587;
                            smtpClient.Credentials = new System.Net.NetworkCredential("penqueapp@gmail.com", "qwknavxpudbjjtfr");

                            smtpClient.Send(mensaje);
                            smtpClient.Dispose();
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("estadisticasPartido/{id}")]
        [ProducesResponseType(typeof(Partido), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> estadisticasPartido(int id)
        {
            var partidos =  _context.Partidos.Include(p => p.predicciones);
            if (partidos == null) return BadRequest("No hay partidos  en la lista");
            Partido partido = new Partido();
            foreach(var aux in partidos){ 
                if(aux.id == id)
                {
                    partido = aux;
                    break;
                }
            }
            int visitante = 0;
            int local = 0;
            int empate = 0;

            foreach(var aux2 in partido.predicciones)
            {
                if(aux2.tipo_Resultado == Tipo_Resultado.Local)
                {
                    local++;
                }
                if (aux2.tipo_Resultado == Tipo_Resultado.Visitante)
                {
                    visitante++;
                }
                if (aux2.tipo_Resultado == Tipo_Resultado.Empate)
                {
                    empate++;
                }
            }
            DtEstadisticas dtE = new DtEstadisticas();
            dtE.local = local;
            dtE.visitante = visitante;
            dtE.empate = empate;

            return Ok(dtE);
        }
    }
}
