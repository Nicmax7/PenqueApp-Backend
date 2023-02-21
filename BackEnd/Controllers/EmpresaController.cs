using BackEnd.Data;
using BackEnd.Models.Clases;
using BackEnd.Models.DataTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {

        private readonly EntidadesDbContext _context;
        public EmpresaController(EntidadesDbContext context) => _context = context;

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id)
        {

            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null) return BadRequest("No existe la empresa");

            DtEmpresa dtEmpresa = new DtEmpresa();
            dtEmpresa.Name = empresa.nombre;
            dtEmpresa.Id = empresa.id;
            dtEmpresa.Billetera = empresa.billetera;
            dtEmpresa.tipo_rol = empresa.tipoRol;
            dtEmpresa.Password = empresa.pass;
            dtEmpresa.Email = empresa.email;

            return empresa == null ? NotFound() : Ok(dtEmpresa);
        }

        [HttpPost("altaEmpresa")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> altaEmpresa(DtEmpresa dtEmpresa)
        {
            DbSet<Usuario> usuarios = _context.Usuario;
            DbSet<Empresa> empresas = _context.Empresas;
            DbSet<Administrador> administradores = _context.Administradores;
            DbSet<SuperAdmin> superadmins = _context.SuperAdmins;
            foreach (var aux in empresas)
            {
                if (aux.email == dtEmpresa.Email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            foreach (var aux in usuarios)
            {
                if (aux.email == dtEmpresa.Email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            foreach (var aux in administradores)
            {
                if (aux.email == dtEmpresa.Email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            foreach (var aux in superadmins)
            {
                if (aux.email == dtEmpresa.Email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            Empresa empresa = new Empresa();
            empresa.email = dtEmpresa.Email;
            empresa.nombre = dtEmpresa.Name;
            empresa.billetera = 0;
            empresa.tipoRol = Tipo_Rol.Admin;
            empresa.pass = dtEmpresa.Password;

            await _context.Empresas.AddAsync(empresa);
            await _context.SaveChangesAsync();

            return Ok(dtEmpresa);
        }

        [HttpGet("misPencas/{id}")]
        [ProducesResponseType(typeof(Equipo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> misPencas(int id)
        {
            List<DtPencaEmpresa> dtPencasEmpresa = new List<DtPencaEmpresa>();
            Empresa empresa = new Empresa();
            var empresas = _context.Empresas.Include(e => e.pencas_empresa);
            foreach (var aux in empresas)
            {
                if (aux.id == id)
                {
                    foreach (var aux2 in aux.pencas_empresa)
                    {
                        if (aux2.estado) {
                            DtPencaEmpresa dtPE = new DtPencaEmpresa();
                            dtPE.id = aux2.id;
                            dtPE.nombre = aux2.nombre;
                            dtPE.tipoPlan = aux2.tipo_Plan;
                            dtPE.entrada = aux2.entrada;
                            dtPE.premioFinal = aux2.pozo;
                            dtPE.tipoDeporte = aux2.tipo_Deporte;
                            dtPE.tipo_Liga = aux2.tipo_Liga;
                            dtPencasEmpresa.Add(dtPE);

                        }
                    }
                    return Ok(dtPencasEmpresa);
                }
            }
            return BadRequest();
        }

        [HttpPut("enviarMensaje")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> enviarMensaje(DtMensaje message)
        {
            int idEmpresa = message.IdEmpresa;
            int idUsuario = message.IdUsuario;
            string mensaje = message.Mensaje;


            var empresa = await _context.Empresas.FindAsync(idEmpresa);
            if (empresa == null) return BadRequest("La empresa no existe");

            var usuario = await _context.Usuario.FindAsync(idUsuario);
            if (usuario == null) return BadRequest("El usuario no existe");

            var todoChats = _context.Chats.Include(e => e.empresa).Include(u => u.usuario).ToList();
            Chat chat = new Chat();
            foreach(var c in todoChats)
            {
                if(c.usuario.id == usuario.id && c.empresa.id == empresa.id)
                {
                    chat = c;
                    Mensaje me = new Mensaje();
                    me.mensaje = "■" + mensaje;//alt + 254
                    await _context.Mensajes.AddAsync(me);

                    chat.mensajes.Add(me);
                    _context.Entry(chat).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return NoContent();
                }
            }


            if (empresa.chats == null)
            {
                empresa.chats = new List<Chat>();
            }

            if (usuario.chats == null)
            {
                usuario.chats = new List<Chat>();
            }

            
            chat.empresa = empresa;
            chat.usuario = usuario;

            Mensaje m = new Mensaje();
            m.mensaje = "■" + mensaje;//alt + 254
            await _context.Mensajes.AddAsync(m);

            chat.mensajes.Add(m);

            usuario.chats.Add(chat);

            empresa.chats.Add(chat);

            await _context.Chats.AddAsync(chat);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("mostrarChats")]
        [ProducesResponseType(typeof(Mensaje), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetChatsById(int idEmpresa)
        {
            var empresa = _context.Empresas.Include(e => e.chats);
            Empresa empresaAux = new Empresa();
            foreach (var aux in empresa)
            {
                if (aux.id == idEmpresa)
                {
                    empresaAux = aux;
                    break;
                }
            }
            List<Chat> chats = empresaAux.chats;
            List<DtChat> chat2 = new List<DtChat>();

            var ch = _context.Chats.Include(c => c.usuario);

            List<Mensaje> mensajes = new List<Mensaje>();
            foreach (var auxC in chats)
            {
                foreach (var auxU in ch)
                {
                    if (auxU.Id == auxC.Id)
                    {
                        DtChat chat = new DtChat();
                        chat.Id = auxC.Id;
                        chat.usuario = auxU.usuario.nombre;
                        chat.empresa = auxC.empresa.nombre;
                        chat.idusuario = auxU.usuario.id;
                        chat.idempresa = auxC.empresa.id;
                        chat2.Add(chat);
                    }
                }
            }
            return Ok(chat2);
        }

        [HttpGet("mostrarMensajes")]
        [ProducesResponseType(typeof(Mensaje), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMensajeById(int idChat)
        {
            var chat = _context.Chats.Include(c => c.mensajes);
            Chat chatAux = new Chat();
            foreach (var aux in chat)
            {
                if (aux.Id == idChat)
                {
                    chatAux = aux;
                    break;
                }
            }
            List<Mensaje> mensajes = chatAux.mensajes;
            return Ok(chatAux.mensajes);
        }

        [HttpPut("depositarBilleteraEmpresa/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> depositarBilleteraEmpresa(int id,int monto)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null) return BadRequest("La Empresa no existe");
            empresa.agregarFondos(monto);
            _context.Entry(empresa).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("invitarUsuario")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> invitarUsuario(DtInvitacion dtInvitacion)
        {
            var user = _context.Usuario.Include(p => p.puntos_por_penca).ToList();
            var penca = await _context.Pencas.FindAsync(dtInvitacion.idPenca);
            var pencas = _context.Pencas.Include(p => p.participantes_puntos).ToList();
            Penca pencaAux = new Penca();
            if (penca == null) return BadRequest("No existe la penca");
            var empresaPenca = _context.Empresas.Include(e => e.pencas_empresa);
            bool empresa = false;
            bool p = false;
            string nombreEmpresa = null;

            

            foreach(var aux in pencas)
            {
                if(aux.id == dtInvitacion.idPenca)
                {
                    pencaAux = aux;
                    break;
                }
            }
            foreach (var aux in empresaPenca)
            {
                if (aux.id == dtInvitacion.idEmpresa)
                {
                    nombreEmpresa = aux.nombre;
                    empresa = true;
                    foreach (var aux2 in aux.pencas_empresa)
                    {
                        if (aux2.id == dtInvitacion.idPenca)
                        {
                            p = true;
                        }
                    }
                }
            }
            if (!empresa)
            {
                return BadRequest("No existe la empresa.");
            }
            if (!p)
            {
                return BadRequest("La Penca no está asociada a la empresa.");
            }

            Usuario usr = new Usuario();
            foreach (var aux in user)
            {
                foreach(var puntos in penca.participantes_puntos)
                {
                    if (aux.puntos_por_penca.Contains(puntos) && aux.email == dtInvitacion.emailUsr) return BadRequest("El usuario ya ha sido invitado");
                }

                if (aux.email == dtInvitacion.emailUsr)
                {
                    
                    usr = aux;
                    Puntuacion puntuacion = new Puntuacion();
                    puntuacion.penca = penca;
                    puntuacion.estado = estado_Penca.Invitado;
                    puntuacion.puntos = 0;

                    string texto = "Saludos " + usr.nombre + ", le informamos que la empresa " + nombreEmpresa + " lo/la ha invitado a formar parte de una penca.";
                    MailMessage mensaje = new MailMessage("penqueapp@gmail.com", dtInvitacion.emailUsr, "[PenqueApp] Ha sido invitado a una penca.", texto);
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new System.Net.NetworkCredential("penqueapp@gmail.com", "qwknavxpudbjjtfr");

                    smtpClient.Send(mensaje);
                    smtpClient.Dispose();

                    if (penca.participantes_puntos == null)
                    {
                        penca.participantes_puntos = new List<Puntuacion>();
                    }

                    if (usr.puntos_por_penca == null)
                    {
                        usr.puntos_por_penca = new List<Puntuacion>();
                    }
                    penca.participantes_puntos.Add(puntuacion);
                    usr.puntos_por_penca.Add(puntuacion);

                    _context.Pencas.Add(penca).State = EntityState.Modified;
                    _context.Usuario.Add(usr).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return NoContent();
                }
            }
            return BadRequest("El usuario no existe");
        }

        [HttpGet("listarUsuariosAEsperaDeConfirmacion")]
        [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> listarUsuariosAEsperaDeConfirmacion(int idEmpresa, int idPenca)
        {
            var penca = await _context.Pencas.FindAsync(idPenca);
            if (penca == null) return BadRequest("no existe la penca");
            var empresas = _context.Empresas.Include(e => e.pencas_empresa).ToList();
            var puntuaciones = _context.Puntuaciones.Include(p => p.usuario).Include(p => p.penca);

            foreach (var empresa in empresas)
            {
                if (empresa.id == idEmpresa)
                {
                    if (empresa.pencas_empresa.Contains(penca)) break;
                    else return BadRequest("la penca no pertenece a esta empresa");
                }
            }

            List<DtUsuario> usuariosPendientes = new List<DtUsuario>();
            foreach (var puntuacion in puntuaciones)
            {
                foreach (var empresa in empresas)
                {
                    if (puntuacion.penca.id == idPenca)
                    {
                        if (puntuacion.estado == estado_Penca.Pendiente)
                        {
                            var DtUsuario = new DtUsuario();
                            DtUsuario.Id = puntuacion.usuario.id;
                            DtUsuario.email = puntuacion.usuario.email;
                            DtUsuario.Nombre = puntuacion.usuario.nombre;
                            usuariosPendientes.Add(DtUsuario);
                        }
                        break;
                    }
                } 
            }
            return Ok(usuariosPendientes);
        }

        [HttpPost("AceptarORechazarUsuario")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AceptarORechazarUsuario(int idUsuario, int idPenca, int idEmpresa, bool aceptar)
        {
            var pencas = _context.Pencas.Include(p => p.liga_Individual).ToList();
            var empresas = _context.Empresas.Include(e => e.pencas_empresa).ToList();
            var puntuaciones = _context.Puntuaciones.Include(p => p.usuario).Include(p => p.penca).ToList();
            var chats = _context.Chats.Include(p => p.usuario).Include(e => e.empresa).ToList();
            Empresa emp = new Empresa();
            Penca p = new Penca();
            var existeEmpresa = false;
            foreach (var empresa in empresas)
            {
                if (empresa.id == idEmpresa)
                {
                    emp = empresa;
                    existeEmpresa = true;
                    var existePenca = false;
                    foreach (var penca in pencas)
                    {
                        if (penca.id == idPenca)
                        {
                            existePenca = true;
                            p=penca;
                            if (empresa.pencas_empresa.Contains(penca)){
                                if (penca.tipo_Plan == Tipo_Plan.Basico && penca.topeUsuarios == 0) return BadRequest("Has superado el maximo de participantes en la Penca");
                                if (penca.tipo_Plan == Tipo_Plan.Basico) penca.topeUsuarios--;
                                _context.Entry(penca).State = EntityState.Modified;
                                break;
                            }
                            else return BadRequest("la penca no pertenece a esta empresa");
                        }
                    }
                    if (!existePenca) return BadRequest("No existe la penca");
                }
            }
            if (!existeEmpresa) return BadRequest("No existe la empresa");

            var usuarioPertenece = false;
            foreach (var puntuacion in puntuaciones)
            {
                if (puntuacion.penca.id == idPenca && puntuacion.usuario.id == idUsuario)
                {
                    usuarioPertenece = true;
                    if (puntuacion.estado == estado_Penca.Pendiente)
                    {
                        if (aceptar){
                            puntuacion.estado = estado_Penca.Aceptado;

                            if (puntuacion.usuario.chats == null)
                            {
                                puntuacion.usuario.chats = new List<Chat>();
                            }
                            if (emp.chats == null)
                            {
                                emp.chats = new List<Chat>();
                            }
                            bool tieneChat = false;
                            foreach(var c in chats)
                            {
                                if(c.usuario.id == puntuacion.usuario.id && c.empresa.id == emp.id)
                                {
                                    tieneChat = true;
                                    break;
                                }
                            }

                            if (!tieneChat) {
                                Chat chat = new Chat();
                                puntuacion.usuario.chats.Add(chat);
                                emp.chats.Add(chat);
                                await _context.Chats.AddAsync(chat);
                            }
                            

                            string texto = "Saludos " + puntuacion.usuario.nombre + ", le avisamos que la penca " + p.nombre + " ha aceptado su solicitud de entrar a jugar.";
                            MailMessage mensaje = new MailMessage("penqueapp@gmail.com", puntuacion.usuario.email, "[PenqueApp] Bienvenido a la Penca de la Empresa", texto);
                            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                            smtpClient.EnableSsl = true;
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Host = "smtp.gmail.com";
                            smtpClient.Port = 587;
                            smtpClient.Credentials = new System.Net.NetworkCredential("penqueapp@gmail.com", "qwknavxpudbjjtfr");

                            smtpClient.Send(mensaje);
                            smtpClient.Dispose();
                        }
                        
                        else {
                            puntuacion.estado = estado_Penca.Rechazado;
                            puntuacion.usuario.billetera += puntuacion.penca.entrada;

                            string texto = "Saludos " + puntuacion.usuario.nombre + ", le avisamos que la penca " + p.nombre + " rechazo su solicitud de ingreso.";
                            MailMessage mensaje = new MailMessage("penqueapp@gmail.com", puntuacion.usuario.email, "[PenqueApp] Boletín informativo", texto);
                            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                            smtpClient.EnableSsl = true;
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Host = "smtp.gmail.com";
                            smtpClient.Port = 587;
                            smtpClient.Credentials = new System.Net.NetworkCredential("penqueapp@gmail.com", "qwknavxpudbjjtfr");

                            smtpClient.Send(mensaje);
                            smtpClient.Dispose();
                        }
                        
                        _context.Puntuaciones.Add(puntuacion).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        break;
                    }
                    else
                    {
                        return BadRequest("El usuario no esta pendiente de aceptación");
                    }
                }
            }
            if (usuarioPertenece) return NoContent();
            else return BadRequest("el usuario no pertenece a la penca");
        }

        [HttpGet("miBilletera/{id}")]
        [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> getBilletera(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null) return BadRequest("No existe el usuario");

            DtEmpresa dtEmpresa = new DtEmpresa();
            dtEmpresa.Billetera = empresa.billetera;
            dtEmpresa.Id = empresa.id;

            return empresa == null ? NotFound() : Ok(dtEmpresa);
        }

        [HttpGet("reportes/{id}")]
        [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> getReportes(int id)
        {
            var empresa = await _context.Empresas.Include(e => e.pencas_empresa).FirstOrDefaultAsync(e => e.id == id);
            if (empresa == null) return BadRequest("No existe el usuario");

            DtReporteEmpresa dtReporteEmpresa = new DtReporteEmpresa();
            var listPencas = new List<DtPenca>();
            dtReporteEmpresa.numeroDePencas = empresa.pencas_empresa.Count();
            dtReporteEmpresa.premiosRepartidos = 0;

            foreach (var aux in empresa.pencas_empresa)
            {
                var penca = await _context.Pencas.Include(p => p.participantes_puntos).Include(p => p.foro).FirstOrDefaultAsync(p => p.id ==  aux.id);
                if (penca == null) return BadRequest("No existe la penca");

                if (penca.estado) 
                {
                    dtReporteEmpresa.pencasActivas++;

                    DtPenca dtPenca = new DtPenca();
                    dtPenca.nombre = penca.nombre;
                    dtPenca.tipo_Plan = penca.tipo_Plan;
                    dtPenca.tipo_Deporte = penca.tipo_Deporte;
                    dtPenca.tipo_Liga = penca.tipo_Liga;
                    dtPenca.fecha_Creacion = penca.fecha_Creacion;
                    dtPenca.pozo = penca.pozo;
                    dtPenca.entrada = penca.entrada;

                    listPencas.Add(dtPenca);
                }
                else
                {
                    dtReporteEmpresa.pencasFinalizadas++;
                    dtReporteEmpresa.premiosRepartidos += penca.pozo;
                }

                if (penca.tipo_Plan == Tipo_Plan.Premium)
                {
                    dtReporteEmpresa.pencasPremium++;
                }

                if (penca.tipo_Liga == Tipo_Liga.Individual)
                {
                    dtReporteEmpresa.pencasIndividuales++;
                }
                else {
                    dtReporteEmpresa.pencasDeEquipo++;
                }

                dtReporteEmpresa.mensajesEnForos += penca.foro.Count();

                dtReporteEmpresa.numeroDeUsuarios += penca.participantes_puntos.Count();

            }

            dtReporteEmpresa.pencas = listPencas.ToArray();

            return empresa == null ? NotFound() : Ok(dtReporteEmpresa);
        }
    }
}
