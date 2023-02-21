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
    public class AdminController : ControllerBase
    {
        private readonly EntidadesDbContext _context;
        public AdminController(EntidadesDbContext context) => _context = context;

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id)
        {

            var admin = await _context.Administradores.FindAsync(id);
            if (admin == null) return BadRequest("No existe el administrador");

            DtAdmin dtAdmin = new DtAdmin();
            dtAdmin.Name = admin.nombre;
            dtAdmin.Id = admin.Id;
            dtAdmin.Billetera = admin.billetera;
            dtAdmin.tipo_rol = admin.Tipo_Rol;
            dtAdmin.Email = admin.email;
            dtAdmin.Password = admin.password;

            return admin == null ? NotFound() : Ok(dtAdmin);
        }

        [HttpPost("altaAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> altaAdmin(DtAdmin dtAdmin)
        {

            DbSet<Usuario> usuarios = _context.Usuario;
            DbSet<Empresa> empresas = _context.Empresas;
            DbSet<Administrador> administradores = _context.Administradores;
            DbSet<SuperAdmin> superadmins = _context.SuperAdmins;
            foreach (var aux in empresas)
            {
                if (aux.email == dtAdmin.Email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            foreach (var aux in usuarios)
            {
                if (aux.email == dtAdmin.Email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            foreach (var aux in administradores)
            {
                if (aux.email == dtAdmin.Email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            foreach (var aux in superadmins)
            {
                if (aux.email == dtAdmin.Email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            Administrador administrador = new Administrador();
            administrador.nombre = dtAdmin.Name;
            administrador.email = dtAdmin.Email;
            administrador.password = dtAdmin.Password;
            administrador.billetera = 0;
            administrador.Tipo_Rol = Tipo_Rol.Admin;

            await _context.Administradores.AddAsync(administrador);
            await _context.SaveChangesAsync();

            return Ok(dtAdmin);
        }
        [HttpPut("agregarPenca")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> agregarPenca(int idADmin, int idPenca)
        {

            var penca = await _context.Pencas.FindAsync(idPenca);
            if (penca == null) return BadRequest("No existe la penca");
            penca.tieneAdmin = true;
            var admin = await _context.Administradores.FindAsync(idADmin);
            if (admin == null) return BadRequest("No existe el administrador");

            if (admin.pencas == null)
            {
                admin.pencas = new List<Penca>();
            }
            admin.pencas.Add(penca);

            _context.Entry(admin).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();

        }
        [HttpGet("PencasCompartidasGet/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> listarPencasCompartidas(int id)
        {

            var admin = _context.Administradores.Include(a => a.pencas).ToList();
            var pencas = _context.Pencas.Include(a => a.liga_Individual).Include(a => a.liga_Equipo).ToList();
            List<DtPencasCompartida> pencasComp = new List<DtPencasCompartida>();
            foreach (var a in admin)
            {
                if (a.Id == id)
                {

                    foreach (var aux in a.pencas)
                    {
                        if (aux.estado){
                            DtPencasCompartida pencasaCompartidas = new DtPencasCompartida();
                            pencasaCompartidas.id = aux.id;
                            pencasaCompartidas.nombre = aux.nombre;
                            pencasaCompartidas.tipoDeporte = aux.tipo_Deporte;
                            pencasaCompartidas.entrada = aux.entrada;
                            pencasaCompartidas.pozo = aux.pozo;
                            pencasaCompartidas.Tipo_Liga = aux.tipo_Liga;
                            pencasaCompartidas.estado = aux.estado;

                            foreach(var p in pencas)
                            {
                                if(aux.tipo_Liga == Tipo_Liga.Individual && p.id == aux.id)
                                {
                                    pencasaCompartidas.idLiga = p.liga_Individual.Id;
                                    pencasaCompartidas.estadoLiga = p.liga_Individual.activa;
                                }
                                if (aux.tipo_Liga == Tipo_Liga.Equipo && p.id == aux.id)
                                {
                                    pencasaCompartidas.idLiga = p.liga_Equipo.id;
                                    pencasaCompartidas.estadoLiga = p.liga_Equipo.activa;
                                }
                            }


                            pencasComp.Add(pencasaCompartidas);
                        }
                        
                    }
                    return Ok(pencasComp);
                }
                
            }
            return NotFound();
        }

        [HttpGet("getAdmins")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getAdmins() { 

            List<DtAdmin> dtAdmins = new List<DtAdmin> ();
            var admins = _context.Administradores;

            foreach (var aux in admins) { 

                DtAdmin dtAdmin = new DtAdmin();
                dtAdmin.Id = aux.Id;
                dtAdmin.Name = aux.nombre;
                dtAdmin.Email = aux.email;
                dtAdmins.Add(dtAdmin);
            }

            return dtAdmins == null ? NotFound() : Ok(dtAdmins);      
        }

        [HttpGet("getPencasCompartidasSinAdminEquipo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getPencasCompartidasSinAdminEquipo(int idLiga)
        {

            var pencas = _context.Pencas.Include(a => a.liga_Equipo).ToList();
            List<DtPencasCompartida> pencasComp = new List<DtPencasCompartida>();

                    foreach (var aux in pencas)
                    {
                        if (aux.estado && !aux.tieneAdmin && aux.tipo_Penca == Tipo_Penca.Compartida && aux.liga_Equipo.id == idLiga)
                        {
                            DtPencasCompartida pencasaCompartidas = new DtPencasCompartida();
                            pencasaCompartidas.id = aux.id;
                            pencasaCompartidas.nombre = aux.nombre;

                            pencasComp.Add(pencasaCompartidas);
                        }

                    }
                    return Ok(pencasComp);
        }

        [HttpGet("getPencasCompartidasSinAdminIndividual")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getPencasCompartidasSinAdminIndividual(int idLiga)
        {

            var pencas = _context.Pencas.Include(a => a.liga_Individual).ToList();
            List<DtPencasCompartida> pencasComp = new List<DtPencasCompartida>();

            foreach (var aux in pencas)
            {
                if (aux.estado && !aux.tieneAdmin && aux.tipo_Penca == Tipo_Penca.Compartida && aux.liga_Individual.Id == idLiga)
                {
                    DtPencasCompartida pencasaCompartidas = new DtPencasCompartida();
                    pencasaCompartidas.id = aux.id;
                    pencasaCompartidas.nombre = aux.nombre;

                    pencasComp.Add(pencasaCompartidas);
                }

            }
            return Ok(pencasComp);
        }
    }
}


