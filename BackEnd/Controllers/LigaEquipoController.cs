using BackEnd.Data;
using BackEnd.Models.Clases;
using BackEnd.Models.DataTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LigaEquipoController : ControllerBase
    {
        private readonly EntidadesDbContext _context;
        public LigaEquipoController(EntidadesDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Liga_Equipo>> Get()
        {
            return await _context.Liga_Equipos.ToListAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Liga_Equipo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id)
        {

            var Liga = await _context.Liga_Equipos.FindAsync(id);
            if (Liga == null) return BadRequest("No existe la Liga");

            DtLigaEquipo dtLiga = new DtLigaEquipo();
            dtLiga.id = Liga.id;
            dtLiga.nombreLiga = Liga.nombreLiga;
            dtLiga.tope = Liga.topePartidos;
            dtLiga.tipoDeporte = Liga.tipoDeporte;

            return Liga == null ? NotFound() : Ok(dtLiga);
        }

        [HttpGet("getLigasSinUsar")]
        [ProducesResponseType(typeof(Liga_Equipo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> getLigasSinUsar()
        {
            var lEs = _context.Liga_Equipos.ToList();
            List<DtLigaEquipo> dtLigaE = new List<DtLigaEquipo>();

            foreach (var aux in lEs)
            {
                if (aux.activa && aux.topePartidos != 0)
                {
                    DtLigaEquipo dtLE = new DtLigaEquipo();
                    dtLE.nombreLiga = aux.nombreLiga;
                    dtLE.tope = aux.topePartidos;
                    dtLE.tipoDeporte = aux.tipoDeporte;
                    dtLE.id = aux.id;

                    dtLigaE.Add(dtLE);
                }
            }
            return dtLigaE == null ? NotFound() : Ok(dtLigaE);
        }

        [HttpGet("getLigasPorDeporte")]
        [ProducesResponseType(typeof(Liga_Equipo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> getLigasPorDeporte(Tipo_Deporte tipo)
        {
            var lEs = _context.Liga_Equipos.ToList();
            List<DtLigaEquipo> dtLigaE = new List<DtLigaEquipo>();

            foreach (var aux in lEs)
            {
                if (aux.activa  && aux.tipoDeporte == tipo)
                {
                    DtLigaEquipo dtLE = new DtLigaEquipo();
                    dtLE.nombreLiga = aux.nombreLiga;
                    dtLE.tope = aux.topePartidos;
                    dtLE.tipoDeporte = aux.tipoDeporte;
                    dtLE.id = aux.id;

                    dtLigaE.Add(dtLE);
                }
            }
            return dtLigaE == null ? NotFound() : Ok(dtLigaE);
        }

        [HttpPost("altaLigaEquipo")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> altaEquipo(DtLigaEquipo dtLigaEquipo)
        {
            Liga_Equipo ligaE = new Liga_Equipo();

            ligaE.nombreLiga = dtLigaEquipo.nombreLiga;
            ligaE.tipoDeporte = dtLigaEquipo.tipoDeporte;
            if(dtLigaEquipo.tope < 3)
            {
                return BadRequest("El tope de la liga debe ser al menos de 3 partidos.");
            }
            ligaE.topePartidos = dtLigaEquipo.tope;
            ligaE.activa = true;

            await _context.Liga_Equipos.AddAsync(ligaE);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = ligaE.id }, ligaE);
        }

        [HttpPut("agregarPartido/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> actualizarResultado(int id, int idPartido)
        {
            var ligaE = await _context.Liga_Equipos.FindAsync(id);
            if (ligaE == null) return BadRequest("No existe la liga");

            if (ligaE.partidos != null){ 
                if(ligaE.topePartidos == 0) {
                    
                    return BadRequest("Ya esta completo todos los slots");
                    
                }
            }
            var partido = await _context.Partidos.FindAsync(idPartido);
            if (partido == null) return BadRequest("No existe el partido");
            if (partido.enUso) return BadRequest("Este partido ya existe en otra liga");
            if (ligaE.partidos == null)
            {
                ligaE.partidos = new List<Partido>();
            }
            ligaE.topePartidos--;
            ligaE.partidos.Add(partido);
            partido.enUso = true;

            _context.Entry(ligaE).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("getPartidos/{id}")]
        [ProducesResponseType(typeof(Liga_Equipo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> getPartidos(int id)
        {
            var ligaE = _context.Liga_Equipos.Include(e => e.partidos).ToList();
            var partidos = _context.Partidos.ToList();

            List<DtPartido> partidosList = new List<DtPartido>();
            
           
            foreach (var item in ligaE)
            {
                if (item.id == id)
                {
                    foreach (var aux in item.partidos)
                    {
                        DtPartido partido = new DtPartido();
                        partido.id = aux.id;
                        partido.fecha = aux.fechaPartido;
                        partido.resultado = aux.resultado;

                        foreach(var e in partidos) {
                            if(aux.id == e.id)
                            {
                                var local = await _context.Equipos.FindAsync(aux.IdLocal);
                                if (local == null) return BadRequest("No existe el equipo");
                                partido.local = local.nombreEquipo;

                                var visitante = await _context.Equipos.FindAsync(aux.IdVisitante);
                                if (visitante == null) return BadRequest("No existe el equipo");
                                partido.visitante = visitante.nombreEquipo;
                                
                                break;
                            }
                        }

                        partidosList.Add(partido);
                    }
                    return Ok(partidosList);
                }
            }

            return ligaE == null ? NotFound() : Ok(ligaE);
        }

        [HttpPut("chequearFinalizada/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> chequearFinalizada(int id)
        {
            var ligas = _context.Liga_Equipos.Include(p => p.partidos);
            Liga_Equipo liga = new Liga_Equipo();
            foreach(var l in ligas)
            {
                if(l.id == id)
                {
                    liga = l;
                    break;
                }
            }
            if (liga == null) return BadRequest("No existe la Liga de Equipos");
            if(liga.topePartidos != 0)
            {
                return BadRequest("A la liga aún le faltan partidos por asignar.");
            }
            
            bool exito = liga.actualizarEstado();
            if (exito) return BadRequest("Todavia hay partidos que no finalizaron");
            _context.Entry(liga).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
