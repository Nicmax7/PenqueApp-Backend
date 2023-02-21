using BackEnd.Data;
using BackEnd.Models.Clases;
using BackEnd.Models.DataTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LigaIndividualController : ControllerBase
    {
        private readonly EntidadesDbContext _context;
        public LigaIndividualController(EntidadesDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Liga_Individual>> Get()
        {
            return await _context.Liga_Individuales.ToListAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Liga_Individual), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id)
        {
            var Liga = await _context.Liga_Individuales.FindAsync(id);
            if (Liga == null) return BadRequest("No existe la Liga");

            DtLiga_Individual dtLiga = new DtLiga_Individual();
            dtLiga.Id = Liga.Id;
            dtLiga.Nombre = Liga.Nombre;
            dtLiga.tipoArea = Liga.tipoArea;
            dtLiga.topeCompetencias = Liga.topeCompetencias;
           

            return Liga == null ? NotFound() : Ok(dtLiga);
        }

        [HttpGet("getLigasSinUsar")]
        [ProducesResponseType(typeof(Liga_Individual), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> getLigasSinUsar()
        {
            var lIs = _context.Liga_Individuales.ToList();
            List<DtLiga_Individual> dtLigaI = new List<DtLiga_Individual>();

            foreach (var aux in lIs)
            {
                if (aux.activa && aux.topeCompetencias > aux.competencias.Count())
                {
                    DtLiga_Individual dtLI = new DtLiga_Individual();
                    dtLI.Nombre = aux.Nombre;
                    dtLI.topeCompetencias = aux.topeCompetencias;
                    dtLI.tipoArea = aux.tipoArea;
                    dtLI.Id = aux.Id;

                    dtLigaI.Add(dtLI);
                }
            }
            return dtLigaI == null ? NotFound() : Ok(dtLigaI);
        }

        [HttpPost("altaLigaIndividual")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> altaLigaI(DtLiga_Individual ligaI)
        {
            Liga_Individual liga = new Liga_Individual();
            liga.Nombre = ligaI.Nombre;
            liga.tipoArea = ligaI.tipoArea;
            if(ligaI.topeCompetencias < 3)
            {
                return BadRequest("La liga debe tener al menos 3 competencias.");
            }
            liga.topeCompetencias = ligaI.topeCompetencias;
            liga.activa = true;
            await _context.Liga_Individuales.AddAsync(liga);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = ligaI.Id }, ligaI);
        }

        [HttpGet("getLigasPorDisciplina")]
        [ProducesResponseType(typeof(Liga_Equipo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> getLigasPorDisciplina(Tipo_Area disciplina)
        {
            var lIs = _context.Liga_Individuales.ToList();
            List<DtLiga_Individual> dtLigaI = new List<DtLiga_Individual>();

            foreach (var aux in lIs)
            {
                if (aux.activa && aux.tipoArea == disciplina)
                {
                    DtLiga_Individual dtLI = new DtLiga_Individual();
                    dtLI.Nombre = aux.Nombre;
                    dtLI.topeCompetencias = aux.topeCompetencias;
                    dtLI.tipoArea = aux.tipoArea;
                    dtLI.Id = aux.Id;

                    dtLigaI.Add(dtLI);
                }
            }
            return dtLigaI == null ? NotFound() : Ok(dtLigaI);
        }

        [HttpPut("agregarCompetencia")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> agregarCompetencia(int idLiga, int idCompetencia)
        {
            var ligaI = await _context.Liga_Individuales.FindAsync(idLiga);
            if (ligaI == null) return BadRequest();

            var ligaIAux = _context.Liga_Individuales.Include(l => l.competencias);
            
            Liga_Individual liga2 = new Liga_Individual();

            List<Competencia> comp = new List<Competencia>();

            foreach(var aux in ligaIAux)
            {
                if (aux.Id == idLiga)
                {
                    liga2 = aux;
                    comp = liga2.competencias;
                    break;

                }
            }

            var competencia = await _context.Competencias.FindAsync(idCompetencia);
            if (competencia == null) return BadRequest("No existe la competencia.");

            if(comp.Count == 0)
            {
                ligaI.competencias = new List<Competencia>();
            }
            if (competencia.ligaI) return BadRequest("Esta Competencia ya esta en otra liga");
            if(comp.Count < ligaI.topeCompetencias)
            {
                competencia.ligaI = true;
                ligaI.competencias.Add(competencia);
                _context.Entry(ligaI).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return BadRequest("La liga ya está llena.");
        }

        [HttpGet("mostrarCompetencias/{id}")]
        [ProducesResponseType(typeof(Equipo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCompetenciasByLiga(int id)
        {
            var ligaI = _context.Liga_Individuales.Include(l => l.competencias);
            Liga_Individual ligaI2 = new Liga_Individual();

            foreach(var aux in ligaI)
            {
                if(aux.Id == id)
                {
                    ligaI2 = aux;
                    break;
                }
            }
            List<DtCompetencia> dtcomp = new List<DtCompetencia>();
            foreach(var dt in ligaI2.competencias)
            {
                DtCompetencia comp = new DtCompetencia();
                comp.Id = dt.Id;
                comp.nombre = dt.nombre;
                comp.Area = dt.Area;
                comp.topeParticipantes = dt.topeParticipantes;
                comp.fecha_competencia = dt.fecha_competencia;
                comp.estado = dt.activa;
                dtcomp.Add(comp);
            }
            return Ok(dtcomp);
        }

        [HttpPut("chequearFinalizada/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> chequearFinalizada(int id)
        {
            var liga = await _context.Liga_Individuales.FindAsync(id);
            if (liga == null) return BadRequest("No existe la Liga Individual");
            var ligaI = _context.Liga_Individuales.Include(li => li.competencias).ToList();
            var CompetenciaIAux = _context.Competencias.Include(c => c.posiciones).ToList();
            List<Competencia> competenciaList = new List<Competencia>();
            foreach (var aux in ligaI)
            {
                if (aux.Id == id)
                {
                    if(aux.competencias.Count() < liga.topeCompetencias)
                    {
                        return BadRequest("A la liga aún le faltan competencias por asignar.");
                    }
                    foreach(var ligaCompetencias in aux.competencias)
                    {
                        foreach(var compPosiciones in CompetenciaIAux)
                        {
                            if(ligaCompetencias.Id == compPosiciones.Id)
                            {
                                competenciaList.Add(compPosiciones);
                            }
                        }
                    }
                }
            }
            if (competenciaList == null) return BadRequest();
            bool exito = liga.actualizarEstado(competenciaList);
       
            if (exito) return BadRequest("Todavia hay competencias que no finalizaron");
            _context.Entry(liga).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
