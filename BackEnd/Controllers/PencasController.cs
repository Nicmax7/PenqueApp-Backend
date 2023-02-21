using BackEnd.Data;
using BackEnd.Models.Clases;
using BackEnd.Models.DataTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Runtime.Intrinsics.X86;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PencaController : ControllerBase
    {
        private readonly EntidadesDbContext _context;
        public PencaController(EntidadesDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Penca>> Get()
        {
            return await _context.Pencas.ToListAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Penca), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id)
        {
            var pencas = await _context.Pencas.FindAsync(id);

            return pencas == null ? NotFound() : Ok(pencas);
        }

        [HttpGet("verInfoPenca/{id}")]
        [ProducesResponseType(typeof(Equipo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> getInfoPenca(int id)
        {
            var penca = await _context.Pencas.FindAsync(id);
            if (penca == null) return BadRequest("No existe la penca");

            DtPenca dtPenca = new DtPenca();
            dtPenca.id = penca.id;
            dtPenca.nombre = penca.nombre;
            dtPenca.tipo_Deporte = penca.tipo_Deporte;
            dtPenca.fecha_Creacion = penca.fecha_Creacion;
            dtPenca.entrada = penca.entrada;
            dtPenca.pozo = penca.pozo;
            dtPenca.tipo_Liga = penca.tipo_Liga;
            dtPenca.color = penca.color;
            dtPenca.tieneAdmin = penca.tieneAdmin;
            return Ok(dtPenca);
        }


        [HttpPost("altaPencaCompartida/equipo")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> altaPencaCompartidaEquipo(DtPencasCompartida dtPC)
        {
            Penca penca = new Penca();

            penca.nombre = dtPC.nombre;
            penca.tipo_Deporte = dtPC.tipoDeporte;
            penca.tipo_Penca = Tipo_Penca.Compartida;
            penca.entrada = dtPC.entrada;
            penca.pozo = 0;
            penca.fecha_Creacion = DateTime.Now;
            penca.estado = true;
            penca.color = "";
            penca.tipo_Liga = Tipo_Liga.Equipo;
            penca.liga_Individual = null;
            penca.tieneAdmin = false;
            var LigaE = await _context.Liga_Equipos.FindAsync(dtPC.idLiga);
            if (LigaE == null) return BadRequest("No existe la liga");
            
            penca.liga_Equipo = LigaE;
            if(LigaE.pencas == null) { LigaE.pencas = new List<Penca>(); }
            LigaE.pencas.Add(penca);
            await _context.Pencas.AddAsync(penca);
            await _context.SaveChangesAsync();

            return Ok(dtPC);
        }

        [HttpPost("altaPencaCompartida/individual")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> altaPencaCompartidaIndividual(DtPencasCompartida dtPC)
        {
            Penca penca = new Penca();

            penca.nombre = dtPC.nombre;
            penca.tipo_Deporte = Tipo_Deporte.Competencia;
            penca.tipo_Penca = Tipo_Penca.Compartida;
            penca.entrada = dtPC.entrada;
            penca.pozo = 0;
            penca.fecha_Creacion = DateTime.Now;
            penca.estado = true;
            penca.color = "";
            penca.tipo_Liga = Tipo_Liga.Individual;
            penca.liga_Equipo = null;
            penca.tieneAdmin = false;
            var LigaI = await _context.Liga_Individuales.FindAsync(dtPC.idLiga);
            if (LigaI == null) return BadRequest("No existe la liga");

            penca.liga_Individual = LigaI;
            if (LigaI.pencas == null) { LigaI.pencas = new List<Penca>(); }
            LigaI.pencas.Add(penca);
            await _context.Pencas.AddAsync(penca);
            await _context.SaveChangesAsync();

            return Ok(dtPC);
        }

        [HttpPost("altaPencaEmpresa/equipo")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> altaPencaEmpresa(DtPencaEmpresa dtPE)
        {

            Penca penca = new Penca();

            penca.nombre = dtPE.nombre;
            penca.tipo_Deporte = dtPE.tipoDeporte;
            penca.tipo_Penca = Tipo_Penca.Empresa;
            penca.entrada = dtPE.entrada;
            penca.pozo = dtPE.premioFinal;
            penca.fecha_Creacion = DateTime.Now;
            penca.estado = true;
            penca.color = "";
            penca.tipo_Liga = Tipo_Liga.Equipo;
            penca.liga_Individual = null;

            var LigaE = await _context.Liga_Equipos.FindAsync(dtPE.idLiga);
            var empresa = await _context.Empresas.FindAsync(dtPE.idEmpresa);
            if (LigaE == null) return BadRequest("No existe la liga");
            if (empresa == null) return BadRequest("No existe la empresa");

            if(dtPE.tipoPlan == Tipo_Plan.Premium){

                if (empresa.billetera < 1000) return BadRequest("No tienes saldo suficiente para realizar esta penca");
                empresa.billetera -= 1000;
                penca.topeUsuarios = -1;
            }
            if (dtPE.tipoPlan == Tipo_Plan.Basico)
            {
                penca.topeUsuarios = 8;
            }
            penca.tipo_Plan = dtPE.tipoPlan;

            if (LigaE != null)
            {
                penca.liga_Equipo = LigaE;
                if (LigaE.pencas == null)
                {
                    LigaE.pencas = new List<Penca>();
                }
                LigaE.pencas.Add(penca);
                
            }

            await _context.Pencas.AddAsync(penca);
            
            if(empresa.pencas_empresa == null)
            {
                empresa.pencas_empresa = new List<Penca>();
            }
            empresa.pencas_empresa.Add(penca);
            _context.Entry(empresa).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(dtPE);
        }

        [HttpPost("altaPencaEmpresa/individual")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> altaPencaEmpresaIndividual(DtPencaEmpresa dtPE)
        {
            Penca penca = new Penca();

            penca.nombre = dtPE.nombre;
            penca.tipo_Deporte = Tipo_Deporte.Competencia;
            penca.tipo_Penca = Tipo_Penca.Empresa;
            penca.entrada = dtPE.entrada;
            penca.pozo = dtPE.premioFinal;
            penca.fecha_Creacion = DateTime.Now;
            penca.estado = true;
            penca.color = "";
            penca.tipo_Liga = Tipo_Liga.Individual;
            penca.liga_Equipo = null;
            penca.tipo_Liga = Tipo_Liga.Individual;

            var LigaI = await _context.Liga_Individuales.FindAsync(dtPE.idLiga);
            var empresa = await _context.Empresas.FindAsync(dtPE.idEmpresa);
            if (LigaI == null) return BadRequest("No existe la liga");
            if (empresa == null) return BadRequest("No existe la empresa");
            if (dtPE.tipoPlan == Tipo_Plan.Premium)
            {

                if (empresa.billetera < 1000) return BadRequest("No tienes saldo suficiente para realizar esta penca");
                empresa.billetera -= 1000;
                    penca.topeUsuarios = -1;
            }
            if (dtPE.tipoPlan == Tipo_Plan.Basico)
            {
                penca.topeUsuarios = 8;
            }
            penca.tipo_Plan = dtPE.tipoPlan;

            if (LigaI != null)
            {
                penca.liga_Individual = LigaI;
                if (LigaI.pencas == null)
                {
                    LigaI.pencas = new List<Penca>();
                }
                LigaI.pencas.Add(penca);

            }

            await _context.Pencas.AddAsync(penca);

            if (empresa.pencas_empresa == null)
            {
                empresa.pencas_empresa = new List<Penca>();
            }
            empresa.pencas_empresa.Add(penca);
            _context.Entry(empresa).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(dtPE);
        }


        [HttpGet("verForo/{id}")]
        [ProducesResponseType(typeof(Penca), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMensajesForo(int id)
        {
            var penca = _context.Pencas.Include(e => e.foro);
            List<string> auxList = new List<string>();

            foreach (var aux in penca)
            {
                if (aux.id == id)
                {
                    foreach (var foros in aux.foro)
                    {
                        auxList.Add(foros.mensaje);
                    }
                    return Ok(auxList);
                }
            }
            return BadRequest("No existe la Penca");
        }

        [HttpPut("chequearLigaEquipoFinalizada/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> chequearLigaEquipoFinalizada(int id)
        {
            var pencas = _context.Pencas.Include(le => le.liga_Equipo).ToList();
            if (pencas == null) return BadRequest("No existen pencas");
            Penca penca = new Penca();
            foreach (var aux in pencas)
            {
                if (aux.id == id)
                {
                    penca = aux;
                    break;
                }
            }
            if (penca == null) return BadRequest("No existe la penca");
            var ligasE = _context.Liga_Equipos.Include(p => p.partidos).ToList();
            Liga_Equipo le = new Liga_Equipo();
            foreach (var aux in ligasE)
            {
                if (aux.id == penca.liga_Equipo.id)
                {
                    le = aux;
                    break;
                }
            }
            if(penca.tipo_Liga != Tipo_Liga.Equipo) return BadRequest("La liga no es de equipo");
            if (le.actualizarEstado()) return BadRequest("La liga aun no ha finalizado");

            penca.chequearEstadoLigaEquipo();

            var pencasPuntaje = _context.Pencas.Include(p => p.participantes_puntos).ToList();
            foreach (var aux in pencasPuntaje)
            {
                if (aux.id == id)
                {
                    penca = aux;
                    break;
                }
            }
            
            List<Puntuacion> posiciones = new List<Puntuacion>();
            posiciones = penca.participantes_puntos;
            posiciones.Sort((x, y) => x.puntos.CompareTo(y.puntos));
            posiciones.Reverse();

            float? pozo = penca.pozo;
            float? comicionAdmin = Convert.ToSingle(pozo * 0.3);
            pozo -= comicionAdmin;

            

            if(penca.tipo_Penca == Tipo_Penca.Compartida) {
                var Admins = _context.Administradores.Include(p => p.pencas).ToList();
                foreach (var a in Admins)
                {
                    if (a.pencas.Contains(penca))
                    {
                        a.billetera += comicionAdmin;
                        break;
                    }
                }
            }
            if(penca.tipo_Penca == Tipo_Penca.Empresa) {
                var Empresas = _context.Empresas.Include(p => p.pencas_empresa).ToList();
                foreach (var e in Empresas)
                {
                    if (e.pencas_empresa.Contains(penca))
                    {
                        e.billetera += comicionAdmin;
                        break;
                    }
                }
            }


            int cantPart = posiciones.Count;
           
            var usuarios = _context.Usuario.Include(p => p.puntos_por_penca).ToList();
            foreach(var u in usuarios)
            {
                foreach(var puntos in u.puntos_por_penca)
                {
                    if (cantPart == 1)
                    {
                        if (posiciones.ElementAt(0).id == puntos.id)
                        {
                            u.agregarFondos(Convert.ToSingle(pozo));
                            _context.Entry(u).State = EntityState.Modified;
                        }
                    }
                    if (cantPart == 2)
                    {
                        if (posiciones.ElementAt(0).id == puntos.id)
                        {
                            u.agregarFondos(Convert.ToSingle(pozo * 0.7));
                            _context.Entry(u).State = EntityState.Modified;
                        }
                        if (posiciones.ElementAt(1).id == puntos.id)
                        {
                            u.agregarFondos(Convert.ToSingle(pozo * 0.3));
                            _context.Entry(u).State = EntityState.Modified;
                        }
                    }
                    if (cantPart >= 3)
                    {
                        if (posiciones.ElementAt(0).id == puntos.id)
                        {
                            u.agregarFondos(Convert.ToSingle(pozo * 0.5));
                            _context.Entry(u).State = EntityState.Modified;
                        }
                        if (posiciones.ElementAt(1).id == puntos.id)
                        {
                            u.agregarFondos(Convert.ToSingle(pozo * 0.3));
                            _context.Entry(u).State = EntityState.Modified;
                        }
                        if (posiciones.ElementAt(2).id == puntos.id)
                        {
                            u.agregarFondos(Convert.ToSingle(pozo * 0.2));
                            _context.Entry(u).State = EntityState.Modified;
                        }
                    }
                }
            }

            foreach (var u in usuarios)
            {
                foreach (var puntos in u.puntos_por_penca)
                {
                    if (penca.participantes_puntos.Contains(puntos))
                    {
                        string texto = "Saludos " + u.nombre + ", le avisamos que la penca " + penca.nombre + " en la que ha apostado finalizó.";
                        MailMessage mensaje = new MailMessage("penqueapp@gmail.com", u.email, "[PenqueApp] Finalizó la competencia esperada.", texto);
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

            _context.Entry(penca).State = EntityState.Modified;
            _context.Entry(le).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("chequearLigaIndividualFinalizada/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> chequearLigaIndividualFinalizada(int id)
        {
            var pencas = _context.Pencas.Include(le => le.liga_Individual).ToList();
            if (pencas == null) return BadRequest("No existen pencas");
            Penca penca = new Penca();
            foreach (var aux in pencas)
            {
                if (aux.id == id)
                {
                    penca = aux;
                    break;
                }
            }
            if (penca == null) return BadRequest("No existe la penca");
            if (penca.tipo_Liga != Tipo_Liga.Individual) return BadRequest("La liga no es individual");
            var ligasI = _context.Liga_Individuales.Include(c => c.competencias).ToList();
            var ligaIAux = _context.Competencias.Include(c => c.posiciones).ToList();
            List<Competencia> competenciaList = new List<Competencia>();
            foreach (var aux in ligasI)
            {
                if (aux.Id == penca.liga_Individual.Id)
                {
                    foreach (var ligaCompetencias in aux.competencias)
                    {
                        foreach (var ligaPosiciones in ligaIAux)
                        {
                            if (ligaCompetencias.Id == ligaPosiciones.Id)
                            {
                                competenciaList.Add(ligaPosiciones);
                            }
                        }
                    }
                }
            }
            Liga_Individual li = new Liga_Individual();
            foreach (var aux in ligasI)
            {
                if (aux.Id == penca.liga_Individual.Id)
                {
                    li = aux;
                    break;
                }
            }
            if (li.actualizarEstado(competenciaList)) return BadRequest("La liga aun no ha finalizado");

            penca.chequearEstadoLigaIndividual();

            var pencasPuntaje = _context.Pencas.Include(p => p.participantes_puntos).ToList();
            foreach (var aux in pencasPuntaje)
            {
                if (aux.id == id)
                {
                    penca = aux;
                    break;
                }
            }

            List<Puntuacion> posiciones = new List<Puntuacion>();
            posiciones = penca.participantes_puntos;
            posiciones.Sort((x, y) => x.puntos.CompareTo(y.puntos));
            posiciones.Reverse();

            float? pozo = penca.pozo;
            float? comicionAdmin = Convert.ToSingle(pozo * 0.3);
            pozo -= comicionAdmin;
            if (penca.tipo_Penca == Tipo_Penca.Compartida)
            {
                var Admins = _context.Administradores.Include(p => p.pencas).ToList();
                foreach (var a in Admins)
                {
                    if (a.pencas.Contains(penca))
                    {
                        a.billetera += comicionAdmin;
                        break;
                    }
                }
            }
            if (penca.tipo_Penca == Tipo_Penca.Empresa)
            {
                var Empresas = _context.Empresas.Include(p => p.pencas_empresa).ToList();
                foreach (var e in Empresas)
                {
                    if (e.pencas_empresa.Contains(penca))
                    {
                        e.billetera += comicionAdmin;
                        break;
                    }
                }
            }

            int cantPart = posiciones.Count;

            var usuarios = _context.Usuario.Include(p => p.puntos_por_penca).ToList();
            foreach (var u in usuarios)
            {
                foreach (var puntos in u.puntos_por_penca)
                {
                    if (cantPart == 1)
                    {
                        if (posiciones.ElementAt(0).id == puntos.id)
                        {
                            u.agregarFondos(Convert.ToSingle(pozo));
                            _context.Entry(u).State = EntityState.Modified;
                        }
                    }
                    if (cantPart == 2)
                    {
                        if (posiciones.ElementAt(0).id == puntos.id)
                        {
                            u.agregarFondos(Convert.ToSingle(pozo * 0.7));
                            _context.Entry(u).State = EntityState.Modified;
                        }
                        if (posiciones.ElementAt(1).id == puntos.id)
                        {
                            u.agregarFondos(Convert.ToSingle(pozo * 0.3));
                            _context.Entry(u).State = EntityState.Modified;
                        }
                    }
                    if (cantPart >= 3)
                    {
                        if (posiciones.ElementAt(0).id == puntos.id)
                        {
                            u.agregarFondos(Convert.ToSingle(pozo * 0.5));
                            _context.Entry(u).State = EntityState.Modified;
                        }
                        if (posiciones.ElementAt(1).id == puntos.id)
                        {
                            u.agregarFondos(Convert.ToSingle(pozo * 0.3));
                            _context.Entry(u).State = EntityState.Modified;
                        }
                        if (posiciones.ElementAt(2).id == puntos.id)
                        {
                            u.agregarFondos(Convert.ToSingle(pozo * 0.2));
                            _context.Entry(u).State = EntityState.Modified;
                        }
                    }
                }
            }
           
            foreach(var u in usuarios)
            {
                foreach(var puntos in u.puntos_por_penca)
                {
                    if (penca.participantes_puntos.Contains(puntos))
                    {
                        string texto = "Saludos " + u.nombre + ", le avisamos que la penca "+ penca.nombre + " en la que ha apostado finalizó.";
                        MailMessage mensaje = new MailMessage("penqueapp@gmail.com", u.email, "[PenqueApp] Finalizó la competencia esperada.", texto);
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


            _context.Entry(penca).State = EntityState.Modified;
            _context.Entry(li).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("PencasCompartidasSegunEstado/{id}")]
        [ProducesResponseType(typeof(Penca), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PencasCompartidasSegunEstado(int id, bool estado)
        {
            var usuarios = _context.Usuario.Include(p => p.puntos_por_penca);
            Usuario usuario = new Usuario();
            var puntuaciones = _context.Puntuaciones.Include(p => p.penca);

            foreach(var aux in usuarios)
            {
                if(aux.id == id)
                {
                    usuario = aux;
                    break;
                }
            }

            List<DtPencasCompartida> auxPencas = new List<DtPencasCompartida>();
            foreach(var u in usuario.puntos_por_penca)
            {
                foreach(var p in puntuaciones)
                {
                    if(u.id == p.id && p.penca.tipo_Penca == Tipo_Penca.Compartida)
                    {
                        DtPencasCompartida dtP = new DtPencasCompartida();
                        dtP.id = p.penca.id;
                        dtP.nombre = p.penca.nombre;
                        dtP.tipoDeporte = p.penca.tipo_Deporte;
                        dtP.entrada = p.penca.entrada;
                        auxPencas.Add(dtP);
                    }
                }
            }

            var pencas = _context.Pencas.ToList();
            List <DtPencasCompartida> listaDePencas = new List<DtPencasCompartida>();
            List<DtPencasCompartida> listaDePencasx2 = new List<DtPencasCompartida>();
            if (pencas != null)
            {
                foreach (var aux in pencas)
                {
                    if (aux.estado == estado && aux.tipo_Penca == Tipo_Penca.Compartida)
                    {
                        DtPencasCompartida dtP = new DtPencasCompartida();
                        dtP.id = aux.id;
                        dtP.nombre = aux.nombre;
                        dtP.tipoDeporte = aux.tipo_Deporte;
                        dtP.entrada = aux.entrada;
                        listaDePencas.Add(dtP);
                    }
                }

                foreach(var aux in listaDePencas)
                {
                    bool encontre = false;
                    foreach(var dtP in auxPencas)
                    {
                        if(aux.id == dtP.id)
                        {
                            encontre = true;
                            break;
                        }
                    }
                    if (!encontre)
                    {
                        listaDePencasx2.Add(aux);
                    }
                }
               
                return Ok(listaDePencasx2);
            }
            return BadRequest("No existe la Penca");
        }
        
        [HttpGet("ListarPosiciones/{id}")]
        [ProducesResponseType(typeof(Penca), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ListarPosiciones(int id)
        {
            var pencas = _context.Pencas.Include(pencas => pencas.participantes_puntos);
            List<DtPuntaje>posiciones = new List<DtPuntaje>();
            int posicionU = 0;
            foreach (var aux in pencas)
            {
                if (aux.id == id)
                {
                    foreach (var puntuacion in aux.participantes_puntos)
                    {
                        posicionU++;
                        var posicion = new DtPuntaje();
                        posicion.idPuntuacion = puntuacion.id;
                        posicion.puntaje = puntuacion.puntos;
                        posicion.posicion = posicionU;
                        posiciones.Add(posicion);
                    }    
                }
            }
            posiciones.Sort((x, y) => x.puntaje.CompareTo(y.puntaje));
            posiciones.Reverse();

            var usuarios = _context.Usuario.Include(usuarios => usuarios.puntos_por_penca);
            foreach (var usuario in usuarios)
            {
                foreach (var posicion in posiciones)
                { 
                    foreach (var puntuacion in usuario.puntos_por_penca)
                    {
                        if (puntuacion.id == posicion.idPuntuacion)
                        {
                            posicion.usuario = usuario.nombre;
                        }
                    }                       
                }
            }
            return Ok(posiciones);
        }

        [HttpGet("getIdLIga/{id}")]
        [ProducesResponseType(typeof(Penca), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> getIdLIga(int id)
        {
            var pencaLI = _context.Pencas.Include(i => i.liga_Individual).ToList();
            var pencaLE = _context.Pencas.Include(e => e.liga_Equipo).ToList();


            foreach(var aux in pencaLI)
            {
                if(aux.id == id && aux.tipo_Liga == Tipo_Liga.Individual)
                {
                    return Ok(aux.liga_Individual.Id);
                }
            }
            foreach (var aux in pencaLE)
            {
                if (aux.id == id && aux.tipo_Liga == Tipo_Liga.Equipo)
                {
                    return Ok(aux.liga_Equipo.id);
                }
            }

            return BadRequest("No tiene una liga asociada :(");
        }

        [HttpPut("cambiarColor/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> cambiarColor(int id, string color)
        {
            var penca = await _context.Pencas.FindAsync(id);
            if (penca == null) return BadRequest("No existe la penca");

            penca.color = color;

            _context.Entry(penca).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
