using BackEnd.Data;
using BackEnd.Models.Clases;
using BackEnd.Models.DataTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Runtime.Intrinsics.X86;
using SmtpClient = System.Net.Mail.SmtpClient;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly EntidadesDbContext _context;
        public UsuarioController(EntidadesDbContext context) => _context = context;

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return BadRequest("No existe el usuario");

            DtUsuario dtUsuario = new DtUsuario();
            dtUsuario.tipo_rol = usuario.tipoRol;
            dtUsuario.email = usuario.email;
            dtUsuario.Nombre = usuario.nombre;
            dtUsuario.billetera = usuario.billetera;
            dtUsuario.Id = usuario.id;

            return usuario == null ? NotFound() : Ok(dtUsuario);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> login(DtLogin dtLogin)
        {
            DbSet<Usuario> users = _context.Usuario;
            DbSet<Empresa> empresa = _context.Empresas;
            DbSet<Administrador> administrador = _context.Administradores;
            DbSet<SuperAdmin> superadmin = _context.SuperAdmins;
            foreach (var aux in users)
            {
                if (aux.email == dtLogin.email)
                {
                    if (aux.password == dtLogin.password)
                    {
                        DtUsuario dtUsuario = new DtUsuario();
                        dtUsuario.Id = aux.id;
                        dtUsuario.tipo_rol = aux.tipoRol;
                        dtUsuario.email = aux.email;
                        dtUsuario.Nombre = aux.nombre;
                        dtUsuario.billetera = aux.billetera;

                        return aux == null ? NotFound() : Ok(dtUsuario);
                    }
                    return BadRequest("Datos incorrectos");
                }    
            }
            foreach (var aux in empresa)
            {
                if (aux.email == dtLogin.email)
                {
                    if (aux.pass == dtLogin.password)
                    {
                        DtEmpresa dtEmpresa = new DtEmpresa();
                        dtEmpresa.Id = aux.id;
                        dtEmpresa.Name = aux.nombre;
                        dtEmpresa.Billetera = aux.billetera;
                        dtEmpresa.Email = aux.email;
                        dtEmpresa.tipo_rol = aux.tipoRol;
                        return aux == null ? NotFound() : Ok(dtEmpresa);
                    }
                    return BadRequest("Datos incorrectos");
                }
            }
            foreach (var aux in administrador)
            {
                if (aux.email == dtLogin.email)
                {
                    if (aux.password == dtLogin.password)
                    {
                        DtAdmin dtAdmin = new DtAdmin();
                        dtAdmin.Id = aux.Id;
                        dtAdmin.Name = aux.nombre;
                        dtAdmin.Email = aux.email;
                        dtAdmin.Billetera = aux.billetera;
                        dtAdmin.tipo_rol = aux.Tipo_Rol;
                        return aux == null ? NotFound() : Ok(dtAdmin);
                    }
                    return BadRequest("Datos incorrectos");
                }
            }
            foreach (var aux in superadmin)
            {
                if (aux.email == dtLogin.email)
                {
                    if (aux.password == dtLogin.password)
                    {
                        DtSuperAdmin dtSAdmin = new DtSuperAdmin();
                        dtSAdmin.Id = aux.Id;
                        dtSAdmin.Name = aux.nombre;
                        dtSAdmin.Email = aux.email;
                        dtSAdmin.Economia = aux.economia;
                        dtSAdmin.tipo_rol = aux.Tipo_Rol;

                        return aux == null ? NotFound() : Ok(dtSAdmin);
                    }
                    return BadRequest("Datos incorrectos");
                }    
            }
            return BadRequest("Datos incorrectos");
        }

        [HttpPost("loginGoogle")]
        [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> loginGoogle(DtGoogle dtG)
        {
            DbSet<Usuario> users = _context.Usuario;
            DbSet<Empresa> empresa = _context.Empresas;
            DbSet<Administrador> administrador = _context.Administradores;
            DbSet<SuperAdmin> superadmin = _context.SuperAdmins;
            foreach (var aux in users)
            {
                if (aux.email == dtG.email)
                {
                   
                        DtUsuario dtUsuario = new DtUsuario();
                        dtUsuario.Id = aux.id;
                        dtUsuario.tipo_rol = aux.tipoRol;
                        dtUsuario.email = aux.email;
                        dtUsuario.Nombre = aux.nombre;
                        dtUsuario.billetera = aux.billetera;

                        return aux == null ? NotFound() : Ok(dtUsuario);
                }
            }
            foreach (var aux in empresa)
            {
                if (aux.email == dtG.email)
                {
                    
                        DtEmpresa dtEmpresa = new DtEmpresa();
                        dtEmpresa.Id = aux.id;
                        dtEmpresa.Name = aux.nombre;
                        dtEmpresa.Billetera = aux.billetera;
                        dtEmpresa.Email = aux.email;
                        dtEmpresa.tipo_rol = aux.tipoRol;
                        return aux == null ? NotFound() : Ok(dtEmpresa);
                 
                }
            }
            foreach (var aux in administrador)
            {
                if (aux.email == dtG.email)
                {
                    
                        DtAdmin dtAdmin = new DtAdmin();
                        dtAdmin.Id = aux.Id;
                        dtAdmin.Name = aux.nombre;
                        dtAdmin.Email = aux.email;
                        dtAdmin.Billetera = aux.billetera;
                        dtAdmin.tipo_rol = aux.Tipo_Rol;
                        return aux == null ? NotFound() : Ok(dtAdmin);
                
                }
            }
            foreach (var aux in superadmin)
            {
                if (aux.email == dtG.email)
                {
                   
                        DtSuperAdmin dtSAdmin = new DtSuperAdmin();
                        dtSAdmin.Id = aux.Id;
                        dtSAdmin.Name = aux.nombre;
                        dtSAdmin.Email = aux.email;
                        dtSAdmin.Economia = aux.economia;
                        dtSAdmin.tipo_rol = aux.Tipo_Rol;

                        return aux == null ? NotFound() : Ok(dtSAdmin);
                }
            }
            Usuario usuario = new Usuario();
            usuario.email = dtG.email;
            usuario.nombre = dtG.nombre;
            usuario.password = usuario.generarContraseña();
            usuario.billetera = 0;
            usuario.tipoRol = Tipo_Rol.Comun;

            await _context.Usuario.AddAsync(usuario);
            await _context.SaveChangesAsync();

            DtUsuario dtU = new DtUsuario();
            dtU.Id = usuario.id;
            dtU.tipo_rol = usuario.tipoRol;
            dtU.email = usuario.email;
            dtU.Nombre = usuario.nombre;
            dtU.billetera = usuario.billetera;

            return Ok(dtU);
        }


        [HttpPost("Registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Registrarse([FromBody] DtRegistro dtRegistro)
        {
            DbSet<Usuario> usuarios = _context.Usuario;
            DbSet<Empresa> empresas = _context.Empresas;
            DbSet<Administrador> administradores = _context.Administradores;
            DbSet<SuperAdmin> superadmins = _context.SuperAdmins;
            foreach (var aux in empresas)
            {
                if (aux.email == dtRegistro.email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            foreach (var aux in usuarios)
            {
                if (aux.email == dtRegistro.email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            foreach (var aux in administradores)
            {
                if (aux.email == dtRegistro.email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            foreach (var aux in superadmins)
            {
                if (aux.email == dtRegistro.email) return BadRequest("Ya existe alguien registrada con ese mail");
            }


            Usuario usuario = new Usuario();
            usuario.email = dtRegistro.email;
            usuario.nombre = dtRegistro.nombre;
            usuario.password = dtRegistro.password;
            usuario.billetera = 0;
            usuario.tipoRol = Tipo_Rol.Comun;

            await _context.Usuario.AddAsync(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = usuario.id }, usuario);
        }

        [HttpPut("recuperarContraseña")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> recuperarContraseña(string email)
        {
            DbSet<Usuario> users = _context.Usuario;
            Usuario usuario = new Usuario();
            foreach (var aux in users)
            {
                if (aux.email == email)
                {
                    usuario = aux;
                }
            }
            if (usuario.email == null) return BadRequest("No existe el usuario");
            usuario.password = usuario.generarContraseña();

 
            string texto = "Hola, aqui te dejamos tu nueva contraseña. Esperemos no te vuelvas a olvidarte de ella :P. Contraseña: " + usuario.password;
            MailMessage mensaje = new MailMessage("penqueapp@gmail.com",email, "[PenqueApp] Nueva contraseña",texto);
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential("penqueapp@gmail.com", "qwknavxpudbjjtfr");

            smtpClient.Send(mensaje);
            smtpClient.Dispose();
            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("unirseALaPenca")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> unirseALaPenca(DtJuego dtJ)
        {
            var usuario = await _context.Usuario.FindAsync(dtJ.idUsuario);
            var penca = await _context.Pencas.FindAsync(dtJ.idPenca);
            if (penca == null) return BadRequest("No existe la penca");
            if (usuario == null) return BadRequest("No existe el usuario");

            if (usuario.billetera < penca.entrada) return BadRequest("El usuario no tiene saldo suficiente");
             usuario.billetera -= penca.entrada;

            if (penca.tipo_Penca != Tipo_Penca.Compartida) return BadRequest("La penca no es de tipo compartida");

            Puntuacion puntuacion = new Puntuacion();
            puntuacion.penca = penca;
            puntuacion.estado = estado_Penca.Aceptado;
            puntuacion.usuario = usuario;
            puntuacion.puntos = 0;

            if (usuario.puntos_por_penca == null)
            {
                usuario.puntos_por_penca = new List<Puntuacion>();
            }
            usuario.puntos_por_penca.Add(puntuacion);

            if (penca.participantes_puntos == null)
            {
                penca.participantes_puntos = new List<Puntuacion>();
            }
            penca.participantes_puntos.Add(puntuacion);
            penca.pozo += penca.entrada;

            _context.Entry(usuario).State = EntityState.Modified;
            _context.Entry(penca).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("predecirUnPartido")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> predecirUnPartido(DtPredicciones dtP)
        {
            var usuario = await _context.Usuario.FindAsync(dtP.idUsuario);
            var penca = await _context.Pencas.FindAsync(dtP.idPenca);
            var partido = await _context.Partidos.FindAsync(dtP.idPartido);
            if (penca == null) return BadRequest("No existe la penca");
            if (partido == null) return BadRequest("No existe el partido");
            if (usuario == null) return BadRequest("No existe el usuario");

            Prediccion prediccion = new Prediccion();
            prediccion.partido = partido;
            prediccion.tipo_Resultado = dtP.tipo;
            prediccion.usuario = usuario;
            prediccion.idPenca = dtP.idPenca;

            Usuario user = new Usuario();
            Penca penca2 = new Penca();

            var usuarios = _context.Usuario.Include(e => e.puntos_por_penca);
            var pencas = _context.Pencas.Include(e => e.participantes_puntos);
            foreach (var aux in usuarios)
            {
                if (aux.id == dtP.idUsuario)
                {
                    user = aux;
                    break;
                }
            }
            foreach (var aux in pencas)
            {
                if (aux.id == dtP.idPenca)
                {
                    penca2 = aux;
                    break;
                }
            }

            foreach (var u in user.puntos_por_penca)
            {
                foreach (var p in penca2.participantes_puntos)
                {
                    if (u.id == p.id)
                    {
                        prediccion.idPuntuacionUsuario = p.id;
                        break;
                    }
                }
            }

            if (usuario.predicciones == null)
            {
                usuario.predicciones = new List<Prediccion>();
            }
            usuario.predicciones.Add(prediccion);

            if (partido.predicciones == null)
            {
                partido.predicciones = new List<Prediccion>();
            }
            partido.predicciones.Add(prediccion);

            _context.Entry(usuario).State = EntityState.Modified;
            _context.Entry(partido).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("misPencas/{id}")]
        [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> misPencas(int id)
        {
            List<DtPenca> dtPencas = new List<DtPenca>();
            Usuario user = new Usuario();

            var usuarios = _context.Usuario.Include(e => e.puntos_por_penca);
            var puntuaciones = _context.Puntuaciones.Include(e => e.penca);
            foreach (var aux in usuarios)
            {
                if (aux.id == id)
                {
                    user = aux;
                    break;
                }
            }

            foreach (var puntos in user.puntos_por_penca)
            {
                foreach (var pencas in puntuaciones)
                {
                    if (pencas.id == puntos.id && pencas.estado == estado_Penca.Aceptado)
                    {
                        DtPenca dtPenca = new DtPenca();
                        dtPenca.id = pencas.penca.id;
                        dtPenca.color = pencas.penca.color;
                        dtPenca.nombre = pencas.penca.nombre;
                        dtPenca.tipo_Deporte = pencas.penca.tipo_Deporte;
                        dtPenca.fecha_Creacion = pencas.penca.fecha_Creacion;
                        dtPenca.tipoPenca = pencas.penca.tipo_Penca;

                        dtPencas.Add(dtPenca);
                    }
                }
            }
            return Ok(dtPencas);
        }

        [HttpPost("apostarUnaCompetencia")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> apostarUnaCompetencia(DtApuesta dtA)
        {
            var usuario = await _context.Usuario.FindAsync(dtA.idUsuario);
            var penca = await _context.Pencas.FindAsync(dtA.idPenca);
            var competencia = await _context.Competencias.FindAsync(dtA.idCompetencia);
            var participante = await _context.Participantes.FindAsync(dtA.idParticipante);
            if (penca == null) return BadRequest("No existe la penca");
            if (competencia == null) return BadRequest("No existe la competencia");
            if (usuario == null) return BadRequest("No existe el usuario");
            if (participante == null) return BadRequest("No existe el participante");

            Apuesta apuesta = new Apuesta();
            apuesta.idGanador = dtA.idParticipante;
            apuesta.usuario = usuario;
            apuesta.competencia = competencia;
            apuesta.idPenca = dtA.idPenca;

            Usuario user = new Usuario();
            Penca penca2 = new Penca();

            var usuarios = _context.Usuario.Include(e => e.puntos_por_penca);
            var pencas = _context.Pencas.Include(e => e.participantes_puntos);
            foreach (var aux in usuarios)
            {
                if (aux.id == dtA.idUsuario)
                {
                    user = aux;
                    break;
                }
            }
            foreach (var aux in pencas)
            {
                if (aux.id == dtA.idPenca)
                {
                    penca2 = aux;
                    break;
                }
            }

            foreach (var u in user.puntos_por_penca)
            {
                foreach (var p in penca2.participantes_puntos)
                {
                    if (u.id == p.id)
                    {
                        apuesta.idPuntuacionUsuario = p.id;
                        break;
                    }
                }
            }

            if (usuario.apuestas == null)
            {
                usuario.apuestas = new List<Apuesta>();
            }
            usuario.apuestas.Add(apuesta);

            if (competencia.apuestas == null)
            {
                competencia.apuestas = new List<Apuesta>();
            }
            competencia.apuestas.Add(apuesta);

            _context.Entry(usuario).State = EntityState.Modified;
            _context.Entry(competencia).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("comentarEnForo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> comentarEnForo(DtMensajeForo dtMensajeForo)
        {
            var penca = await _context.Pencas.FindAsync(dtMensajeForo.IdPenca);
            if (penca == null) return BadRequest("No existe la Penca");
            var usuario = await _context.Usuario.FindAsync(dtMensajeForo.IdUsuario);
            if (usuario == null) return BadRequest("No existe el Usuario");
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = usuario.nombre + ": " + dtMensajeForo.Comentario + ".";
            if (penca.foro == null)
            {
                List<Mensaje> foro = new List<Mensaje>();
            }
            penca.foro.Add(mensaje);
            _context.Entry(penca).State = EntityState.Modified; ;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPut("enviarMensaje")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> enviarMensaje(DtMensaje message)
        {
            int idUsuario = message.IdUsuario;
            int idEmpresa = message.IdEmpresa;
            string mensaje = message.Mensaje;

            var usuario = await _context.Usuario.FindAsync(idUsuario);
            if (usuario == null) return BadRequest("El usuario no existe");

            var empresa = await _context.Empresas.FindAsync(idEmpresa);
            if (empresa == null) return BadRequest("La empresa no existe");

            var todoChats = _context.Chats.Include(e => e.empresa).Include(u => u.usuario).ToList();
            Chat chat = new Chat();
            foreach (var c in todoChats)
            {
                if (c.usuario.id == usuario.id && c.empresa.id == empresa.id)
                {
                    chat = c;
                    Mensaje me = new Mensaje();
                    me.mensaje = "☻" + mensaje;//alt + 254
                    await _context.Mensajes.AddAsync(me);

                    chat.mensajes.Add(me);
                    _context.Entry(chat).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return NoContent();
                }
            }

            if (usuario.chats == null)
            {
                usuario.chats = new List<Chat>();
            }
            if(empresa.chats == null)
            {
                empresa.chats = new List<Chat>();
            }

            chat.usuario = usuario;
            chat.empresa = empresa;

            Mensaje m = new Mensaje();
            m.mensaje = "☻" + mensaje; //alt+258
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
        public async Task<IActionResult> GetChatsById(int idUsuario)
        {
            var usuario = _context.Usuario.Include(u => u.chats);
            Usuario usuarioAux = new Usuario();
            foreach (var aux in usuario)
            {
                if (aux.id == idUsuario)
                {
                    usuarioAux = aux;
                    break;
                }
            }
            List<Chat> chats = usuarioAux.chats;
            List<DtChat> chat2 = new List<DtChat>();

            var ch = _context.Chats.Include(c => c.empresa);

            List<Mensaje> mensajes = new List<Mensaje>();
            foreach (var auxC in chats)
            {
                foreach (var auxE in ch)
                {
                    if(auxE.Id == auxC.Id)
                    {
                        DtChat chat = new DtChat();
                        chat.Id = auxC.Id;
                        chat.empresa = auxE.empresa.nombre;
                        chat.usuario = auxC.usuario.nombre;
                        chat.idusuario = auxC.usuario.id;
                        chat.idempresa = auxE.empresa.id;
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

        [HttpPut("depositarBilleteraUsuario/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> depositarBilleteraUsuario(int id, int monto)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return BadRequest("El usuario no existe");
            usuario.agregarFondos(monto);
            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(usuario);
        }

        [HttpGet("verPrediccionPartido")]
        [ProducesResponseType(typeof(Prediccion), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> verPrediccionPartido(int id, int idPartido,int idPenca)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return BadRequest("No existe el usuario");
            var partido = await _context.Partidos.FindAsync(idPartido);
            if (partido == null) return BadRequest("No existe el partido");

            var predicciones = _context.Predicciones.Include(p => p.partido.predicciones).Include(p => p.usuario);
            foreach (var prediccion in predicciones)
            {
                if (prediccion.partido.id == partido.id && prediccion.usuario.id == id && prediccion.idPenca == idPenca)
                {
                    return Ok(prediccion.tipo_Resultado);
                }
            }
            return Ok(Tipo_Resultado.Indefinido);
        }

        [HttpGet("verPrediccionCompetencia")]
        [ProducesResponseType(typeof(Prediccion), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> verPrediccionCompetencia(int id, int idCompetencia, int idPenca)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return BadRequest("No existe el usuario");
            var competencia = await _context.Competencias.FindAsync(id);
            if (competencia == null) return BadRequest("No existe la competencia");

            var apuestas = _context.Apuestas.Include(a => a.competencia.apuestas).Include(a => a.usuario.apuestas).ToList();
            foreach (var apuesta in apuestas)
            {
                if (apuesta.competencia.Id == idCompetencia && apuesta.usuario.id == usuario.id && apuesta.idPenca == idPenca)
                {
                    var participante = await _context.Participantes.FindAsync(apuesta.idGanador);
                    if (participante == null) return BadRequest("No existe el participante");
                    return Ok(participante.nombre);
                }
            }
            return Ok("");
        }

        [HttpGet("invitacionesPenca/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> listarInvitacionesPenca(int id)
        {

            List<DtPenca> dtPencas = new List<DtPenca>();
            Usuario user = new Usuario();

            var usuarios = _context.Usuario.Include(e => e.puntos_por_penca);
            var puntuaciones = _context.Puntuaciones.Include(e => e.penca);
            var estaUsr = false;
            foreach (var aux in usuarios)
            {
                if (aux.id == id)
                {
                    user = aux;
                    estaUsr = true;
                    break;
                }
            }
            if (estaUsr == false) return BadRequest("El usuario no tiene invitaciones pendientes o no existe");

            foreach (var puntos in user.puntos_por_penca)
            {
                foreach (var pencas in puntuaciones)
                {
                    if (pencas.id == puntos.id && pencas.estado == estado_Penca.Invitado)
                    {
                        DtPenca dtPenca = new DtPenca();
                        dtPenca.id = pencas.penca.id;
                        dtPenca.nombre = pencas.penca.nombre;
                        dtPenca.tipo_Deporte = pencas.penca.tipo_Deporte;
                        dtPenca.fecha_Creacion = pencas.penca.fecha_Creacion;
                        dtPencas.Add(dtPenca);
                    }
                }
            }
            return Ok(dtPencas);

        }

        [HttpPut("aceptarInvitacion/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> aceptarInvitacion(int id, int idPenca)
        {
            var usuarios =_context.Usuario.Include(p => p.puntos_por_penca);
            if (usuarios == null) return BadRequest("No hay usuarios");
            var pencas = _context.Pencas.Include(p => p.participantes_puntos);
            if (pencas == null) return BadRequest("No hay pencas");
            Usuario user = new Usuario();
            Penca penca = new Penca();

            foreach(var up in usuarios)
            {
                if(up.id == id)
                {
                    user = up;
                    break;
                }
            }

            foreach (var p in pencas)
            {
                if (p.id == idPenca)
                {
                    penca = p;
                    break;
                }
            }

            foreach(var puntosP in penca.participantes_puntos)
            {
                foreach(var puntosU in user.puntos_por_penca)
                {
                    if (puntosP.id == puntosU.id)
                    {
                        if (puntosU.estado != estado_Penca.Invitado) return BadRequest("La penca ya ha sido actualizada");
                        if (user.billetera < penca.entrada) return BadRequest("El usuario no tiene saldo suficiente");
                        user.billetera -= penca.entrada;
                        puntosU.estado = estado_Penca.Pendiente;
                        break;
                    }
                }
            }

            _context.Entry(user).State = EntityState.Modified;
            _context.Entry(penca).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("rechazarInvitacion/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> rechazarInvitacion(int id, int idPenca)
        {
            var usuarios = _context.Usuario.Include(p => p.puntos_por_penca);
            if (usuarios == null) return BadRequest("No hay usuarios");
            var pencas = _context.Pencas.Include(p => p.participantes_puntos);
            if (pencas == null) return BadRequest("No hay pencas");
            Usuario user = new Usuario();
            Penca penca = new Penca();

            foreach (var up in usuarios)
            {
                if (up.id == id)
                {
                    user = up;
                    break;
                }
            }

            foreach (var p in pencas)
            {
                if (p.id == idPenca)
                {
                    penca = p;
                    if (penca.tipo_Penca != Tipo_Penca.Empresa) return BadRequest("Esta penca no es de tipo empresa");
                    break;
                }
            }

            foreach (var puntosP in penca.participantes_puntos)
            {
                foreach (var puntosU in user.puntos_por_penca)
                {
                    if (puntosP.id == puntosU.id)
                    {
                        if (puntosU.estado != estado_Penca.Invitado) return BadRequest("La penca ya ha sido actualizada");
                        puntosU.estado = estado_Penca.Rechazado;
                        break;
                    }
                }
            }

            _context.Entry(user).State = EntityState.Modified;
            _context.Entry(penca).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("miBilletera/{id}")]
        [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> getBilletera(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return BadRequest("No existe el usuario");

            DtUsuario dtUsuario = new DtUsuario();
            dtUsuario.billetera = usuario.billetera;
            dtUsuario.Id = usuario.id;

            return usuario == null ? NotFound() : Ok(dtUsuario);
        }
    }
    
}
