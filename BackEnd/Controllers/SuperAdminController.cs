using BackEnd.Data;
using BackEnd.Models.Clases;
using BackEnd.Models.DataTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperAdminController : ControllerBase
    {
        private readonly EntidadesDbContext _context;
        public SuperAdminController(EntidadesDbContext context) => _context = context;

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id) { 
        
            var superAdmin = await _context.SuperAdmins.FindAsync(id);
            if (superAdmin == null) return BadRequest("No existe el super administrador");

            DtSuperAdmin dtSuperAdmin = new DtSuperAdmin();
            dtSuperAdmin.Name = superAdmin.nombre;
            dtSuperAdmin.Id = superAdmin.Id;
            dtSuperAdmin.tipo_rol = superAdmin.Tipo_Rol;
            dtSuperAdmin.Email = superAdmin.email;
            dtSuperAdmin.Password = superAdmin.password;
            dtSuperAdmin.Economia = superAdmin.economia;

            return superAdmin == null ? NotFound() : Ok(dtSuperAdmin);
        }

        [HttpPost("altaSuperAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> altaSuperAdmin(DtSuperAdmin dtSuperAdmin)
        {

            DbSet<Usuario> usuarios = _context.Usuario;
            DbSet<Empresa> empresas = _context.Empresas;
            DbSet<Administrador> administradores = _context.Administradores;
            DbSet<SuperAdmin> superadmins = _context.SuperAdmins;
            foreach (var aux in empresas)
            {
                if (aux.email == dtSuperAdmin.Email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            foreach (var aux in usuarios)
            {
                if (aux.email == dtSuperAdmin.Email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            foreach (var aux in administradores)
            {
                if (aux.email == dtSuperAdmin.Email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            foreach (var aux in superadmins)
            {
                if (aux.email == dtSuperAdmin.Email) return BadRequest("Ya existe alguien registrada con ese mail");
            }

            SuperAdmin superAdmin = new SuperAdmin();
            superAdmin.nombre = dtSuperAdmin.Name;
            superAdmin.email = dtSuperAdmin.Email;
            superAdmin.password = dtSuperAdmin.Password;
            superAdmin.economia = 0;
            superAdmin.Tipo_Rol = Tipo_Rol.SuperAdmin;

            await _context.SuperAdmins.AddAsync(superAdmin);
            await _context.SaveChangesAsync();

            return Ok(dtSuperAdmin);
        }
        [HttpGet("PencasCompartidasGet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> listarPencasCompartidas(Tipo_Deporte deporte)
        {
            var pencas = _context.Pencas.Include(a => a.liga_Individual).Include(a => a.liga_Equipo).ToList();
            List<DtPencasCompartida> pencasComp = new List<DtPencasCompartida>();
      
                    foreach (var aux in pencas)
                    {
                        if (aux.estado && aux.tipo_Deporte == deporte)
                        {
                            DtPencasCompartida pencasaCompartidas = new DtPencasCompartida();
                            pencasaCompartidas.id = aux.id;
                            pencasaCompartidas.nombre = aux.nombre;
                            pencasaCompartidas.tipoDeporte = aux.tipo_Deporte;
                            pencasaCompartidas.entrada = aux.entrada;
                            pencasaCompartidas.pozo = aux.pozo;
                            pencasaCompartidas.Tipo_Liga = aux.tipo_Liga;
                            pencasaCompartidas.estado = aux.estado;
                            pencasaCompartidas.tipoPenca = aux.tipo_Penca;

                            if (aux.tipo_Liga == Tipo_Liga.Individual)
                            {
                                pencasaCompartidas.idLiga = aux.liga_Individual.Id;
                                pencasaCompartidas.estadoLiga = aux.liga_Individual.activa;
                            }
                           if (aux.tipo_Liga == Tipo_Liga.Equipo)
                           {
                            pencasaCompartidas.idLiga = aux.liga_Equipo.id;
                            pencasaCompartidas.estadoLiga = aux.liga_Equipo.activa;
                           }
                            


                            pencasComp.Add(pencasaCompartidas);
                        }

                    }
                    return Ok(pencasComp);
            return NotFound();
        }

        /*[HttpPost("meterDatos")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public void meterDatos()
        {
            try
            {
                SqlConnection conexion = new SqlConnection("Data Source=penqueappserver.database.windows.net;Initial Catalog=PenqueAppDB;User ID=UsuarioPenqueApp;Password=1234PenqueApp2022;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
                conexion.Open();
                string cadena = "insert into participantes(nombre,Area,pais) values ('El Taliban', 0, 'Afganistán')";
                SqlCommand comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre,Area,pais) values ('Diego Gallo González', 0, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre,Area,pais) values ('Dante Iocco', 0, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Paul Kutscher', 0, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Julio César Maglione', 0, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Gabriel Melconian Alvez', 0, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Wilfredo Raymondo', 0, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Carlos Scanavino', 0, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Alberto Zorrilla', 0, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Luis Alberto Nicolao', 0, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('José Meolans', 0, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Federico Grabich', 0, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Arnulfo Carrera', 0, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Carlos Cetino', 0, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Edgar Raúl Culajay', 0, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Leonel López', 0, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Orlando Alvarez', 0, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Fidel Velasquez ', 0, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Armando Pérez', 0, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Emigdio García ', 0, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Henry Isaías Pérez ', 0, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Juan Alfonso Samayoa', 0, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Socorro Chajón', 1, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();       
                cadena = "insert into participantes(nombre, Area,pais) values ('Isaias Aldana', 1, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Ramon González', 1, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Cesar Orantes', 1, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values (' José Figueroa', 1, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Angel Mansilla', 1, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Angel Cruz', 1, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Moices Cruz', 1, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Marcial López', 1, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Ricardo Sirín', 1, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Francisco Lopez S', 1, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Vicente Tiul', 1, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Leonardo Del Cid', 1, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Vicente Macz', 1, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Alejandro Gil', 1, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Fernando Franco', 1, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Xavier Enriquez', 1, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Francisco Hilto', 1, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Antonio Castro', 1, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Roberto Rohr', 1, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Elias Ajcojon', 1, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Bernardo Moreno', 2, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Carlos Varela', 2, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Cristopher De Alba', 2, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Diego Escobar', 2, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Ricardo Gallo', 2, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Javier Ramírez', 2, 'Uruguay')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Gabriel Ibarra', 2, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Gilberto Hernández', 2, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Javier Hernández', 2, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Manuel Murguia', 2, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Jesus Torres', 2, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Miguel Herrera', 2, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Jorge Pérez', 2, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Gregorio Casera', 2, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Manuel Flora', 2, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Julio Ornelas', 2, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Luisao Hernández', 2, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Manuel Ramirez', 2, 'Argentina')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Miguel Sánchez', 2, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Ronaldo Sestopal', 2, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Ricardo Meoni', 2, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Joao Berta', 2, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into participantes(nombre, Area,pais) values ('Felipe Coimbra', 2, 'Brasil')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Competencias(Area, fecha_competencia, nombre, ligaI, topeParticipantes, activa, Liga_IndividualId) values (0, '2022-10-06', 'Competencia1', 0, 3, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Nombres(nombre, competenciaId) values ('Competencia1', 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Competencias(Area, fecha_competencia, nombre, ligaI, topeParticipantes, activa ,Liga_IndividualId) values (0, '2022-10-06', 'Competencia2', 0, 3, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Nombres(nombre, competenciaId) values ('Competencia2', 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Competencias(Area, fecha_competencia, nombre, ligaI, topeParticipantes, activa, Liga_IndividualId) values (1, '2022-10-06', 'Competencia3', 0, 5, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Nombres(nombre, competenciaId) values ('Competencia3', 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Competencias(Area, fecha_competencia, nombre, ligaI, topeParticipantes, activa, Liga_IndividualId) values (1, '2022-10-06', 'Competencia4', 0, 8, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                /*cadena = "insert into Nombres(nombre, competenciaId) values ('Competencia4', 4)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Competencias(Area, fecha_competencia, nombre, ligaI, topeParticipantes, activa, Liga_IndividualId) values (2, '2022-10-06', 'Competencia5', 0, 3, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                /*cadena = "insert into Nombres(nombre, competenciaId) values ('Competencia5', 5)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Competencias(Area, fecha_competencia, nombre, ligaI, topeParticipantes, activa, Liga_IndividualId) values (2, '2022-10-06', 'Competencia6', 0, 4, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                /*cadena = "insert into Nombres(nombre, competenciaId) values ('Competencia6', 6)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Competencias(Area, fecha_competencia, nombre, ligaI, topeParticipantes, activa, Liga_IndividualId) values (0, '2022-10-06', 'Competencia7', 0, 3, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Competencias(Area, fecha_competencia, nombre, ligaI, topeParticipantes, activa, Liga_IndividualId) values (0, '2022-10-06', 'Competencia8', 0, 3, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Competencias(Area, fecha_competencia, nombre, ligaI, topeParticipantes, activa, Liga_IndividualId) values (1, '2022-10-06', 'Competencia9', 0, 3, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Competencias(Area, fecha_competencia, nombre, ligaI, topeParticipantes, activa, Liga_IndividualId) values (1, '2022-10-06', 'Competencia10', 0, 3, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Competencias(Area, fecha_competencia, nombre, ligaI, topeParticipantes, activa, Liga_IndividualId) values (2, '2022-10-06', 'Competencia11', 0, 3, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Competencias(Area, fecha_competencia, nombre, ligaI, topeParticipantes, activa, Liga_IndividualId) values (2, '2022-10-06', 'Competencia12', 0, 3, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Competencias(Area, fecha_competencia, nombre, ligaI, topeParticipantes, activa, Liga_IndividualId) values (1, '2022-10-06', 'Competencia13', 0, 3, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (1, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (1, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (1, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (2, 4)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (2, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (2, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (3, 5)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (3, 6)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (3, 7)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (3, 10)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (3, 11)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (4, 5)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (4, 6)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (4, 7)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (4, 10)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (4, 11)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (4, 12)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (4, 13)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (4, 14)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (5, 8)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (5, 9)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (5, 15)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (6, 8)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (6, 9)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (6, 15)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (6, 16)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (7, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (7, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (7, 4)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (8, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (8, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (8, 4)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (9, 5)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (9, 6)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (9, 7)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (10, 10)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (10, 11)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (10, 12)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (11, 8)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (11, 9)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (11, 16)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (12, 15)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (12, 8)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (12, 9)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (13, 5)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (13, 6)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into CompetenciaParticipante(competenciasId, participantesId) values (13, 7)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Liga_Individuales(Nombre, topeCompetencias, activa, tipoArea) values ('LigaI1', 3, 1, 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Liga_Individuales(Nombre, topeCompetencias, activa, tipoArea) values ('LigaI2', 3, 1, 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Liga_Individuales(Nombre, topeCompetencias, activa, tipoArea) values ('LigaI3', 3, 1, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Liga_Individuales(Nombre, topeCompetencias, activa, tipoArea) values ('LigaI4', 3, 1, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 1 WHERE Id = 1;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 1 WHERE Id = 2;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 1 WHERE Id = 3;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 2 WHERE Id = 7;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 2 WHERE Id = 8;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 2 WHERE Id = 1;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 3 WHERE Id = 9;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 1 WHERE Id = 7;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 3 WHERE Id = 9;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 3 WHERE Id = 10;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 4 WHERE Id = 6;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 4 WHERE Id = 11;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 4 WHERE Id = 12;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 2 WHERE Id = 4;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Competencias SET ligaI = 1, Liga_IndividualId = 3 WHERE Id = 13;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Qatar', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Senegal', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Ecuador', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Paises Bajos', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Inglaterra', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Estados Unidos', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Irán', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Gales', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Argentina', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('México', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Polonia', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Arabia Saudíta', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Dinamarca', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Francia', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Túnez', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Australia', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Alemania', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Japón', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('España', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Costa Rica', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Marruecos', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Canadá', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Bélgica', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Croacia', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Brasil', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Suiza', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Serbia', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Camerún', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Uruguay', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Portugal', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Corea del Sur', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Ghana', 0, 'Internacional', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Peñarol', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Nacional', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Defensor', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Danubio', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Montevideo City', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Montevideo Wanderers', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Plaza Colonia', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Liverpool', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('River Plate', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Deportivo Maldonado', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Boston River', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Cerro Largo', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Fenix ', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Rentistas ', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Cerrito ', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Albion ', 0, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Rampla', 0, 'Uruguay', 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Racing', 0, 'Uruguay', 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('La Luz', 0, 'Uruguay', 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Miramar Misiones', 0, 'Uruguay', 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Progreso', 0, 'Uruguay', 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Atenas', 0, 'Uruguay', 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Juventud', 0, 'Uruguay', 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Sud America', 0, 'Uruguay', 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Uruguay Montevideo', 0, 'Uruguay', 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Central Español', 0, 'Uruguay', 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Villa Española', 0, 'Uruguay', 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Boca', 0, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('River', 0, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('San Lorenzo', 0, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Racing', 0, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Lanúz', 0, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Aldosivi', 0, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Talleres', 0, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Independiente', 0, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Estudiantes', 0, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Newells', 0, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Huracán', 0, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Atletico Tucumán', 0, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Argentinos Jrs.', 0, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Godoy Cruz', 0, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Patronato', 0, 'Argentina', 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Palmerias', 0, 'Brasil', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Flamengo', 0, 'Brasil', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Santos', 0, 'Brasil', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Fluminense', 0, 'Brasil', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Corinthians', 0, 'Brasil', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Paranaense', 0, 'Brasil', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Atletico Minerio', 0, 'Brasil', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Sao Paulo', 0, 'Brasil', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Coritiba', 0, 'Brasil', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Internacional', 0, 'Brasil', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Trouville', 1, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Malvín', 1, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Goes', 1, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Larre Borges', 1, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Biguá', 1, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Aguada', 1, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Nacional', 1, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Hebraica', 1, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Defensor', 1, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Olimpia', 1, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Urunday', 1, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Urupan', 1, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('instituto', 1, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Olímpico', 1, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('San Lorenzo', 1, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('El único cuadro Brasilero', 1, 'Brasil', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('VoleyUy', 3, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('VoleyUy2', 3, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('VoleyBr', 3, 'Brasil', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('VoleyAr', 3, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('VoleyAr2', 3, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('VoleyAr3', 3, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Pablo Cuevas', 2, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('El hermano de Cuevas', 2, 'Uruguay', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Roger Federer', 2, 'Suiza', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Rafael Nadal', 2, 'España', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Andy Murray', 2, 'Escocia', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Carlos Alcaráz', 2, 'España', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Novak Djokovic', 2, 'Serbia', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Casper Ruud', 2, 'Croacia', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Diego Schwartzman', 2, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Francisco Cerundolo', 2, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Equipos(nombreEquipo, deporte, pais, division) values ('Sebastaian Baez', 2, 'Argentina', 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();


                //Mundial
                cadena = "insert into Liga_Equipos(nombreliga, topePartidos, activa, tipoDeporte) values ('Copa Mundial de la FIFA', 64, 1, 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Qatar-Ecuador
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-20', 3, 1, 3, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                
                //Senegal-Holanda
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-21', 3, 2, 4, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Inglaterra - Iran
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-21', 3, 5, 7, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Estados Unidos - Gales
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-22', 3, 6, 8, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Argentina - Arabia
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-22', 3, 9, 12, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Mexico - Polonia
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-22', 3, 10, 11, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Dinamarca - Tunez
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-22', 3, 13, 15, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Francia - Australia
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-23', 3, 14, 16, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Alemania - Japón
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-23', 3, 17, 18, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //España - CRC
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-23', 3, 19, 20, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Marruecos - Croacia
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-23', 3, 21, 24, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Suiza - Camerun
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-24', 3, 26, 28, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Brasil - Serbia
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-24', 3, 25, 27, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Uruguay - Corea del sur
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-24', 3, 29, 31, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Portugal - Ghana
                cadena = "insert into Partidos(fechaPartido, resultado, IdLocal, IdVisitante, enUso, deporte, Liga_EquipoId) values ('2022-11-24', 3, 30, 32, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();


                /*
                ----
                //Qatar-Senegal
                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-11-25', 3, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (3, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (3, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Holanda-Ecuador
                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-11-25', 3, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (4, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (4, 4)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Qatar-Holanda
                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-11-29', 3, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (5, 4)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (5, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                //Ecuador-Senegal
                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-11-29', 3, 1, 0, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (6, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (6, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                */

                /*
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 1 WHERE Id = 2;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 1 WHERE Id = 3;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 1 WHERE Id = 4;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 1 WHERE Id = 5;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 1 WHERE Id = 6;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();*/
                /*
                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 0, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (1, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (1, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 0, NULL)"; 
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (2, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (2, 4)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 0, NULL)"; 
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (3, 5)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (3, 6)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 0, NULL)"; 
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (4, 7)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (4, 8)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 0, NULL)"; 
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (5, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (5, 4)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 0, NULL)"; 
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (6, 8)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (6, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 0, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (7, 7)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (7, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 0, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (8, 6)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (8, 5)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (9, 9)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (9, 10)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (10, 11)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (10, 12)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (11, 13)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (11, 14)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (12, 15)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (12, 16)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (13, 12)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (13, 10)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (14, 15)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (14, 9)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (15, 11)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (15, 13)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 1, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (16, 16)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (16, 12)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 2, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (17, 17)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (17, 18)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 2, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (18, 19)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (18, 20)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 2, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (19, 21)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (19, 22)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 2, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (20, 23)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (20, 24)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 2, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (21, 21)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (21, 18)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 2, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (22, 20)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (22, 17)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 2, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (23, 19)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (23, 23)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 2, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (24, 24)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (24, 18)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 3, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (25, 25)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (25, 26)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 3, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (26, 27)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (26, 28)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 3, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (27, 29)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (27, 30)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 3, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (28, 31)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (28, 32)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 3, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (29, 26)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (29, 29)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 3, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (30, 30)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (30, 25)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 3, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (31, 31)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (31, 28)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "insert into Partidos(fechaPartido, resultado, enUso, deporte, Liga_EquipoId) values ('2022-10-06', 3, 0, 3, NULL)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (32, 27)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into EquipoPartido(partidosId, visitante_localId) values (32, 32)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                */
                /*
                cadena = "insert into Liga_Equipos(nombreliga, topePartidos, activa, tipoDeporte) values ('LigaE1', 5, 1, 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Liga_Equipos(nombreliga, topePartidos, activa, tipoDeporte) values ('LigaE2', 3, 1, 0)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Liga_Equipos(nombreliga, topePartidos, activa, tipoDeporte) values ('LigaE3', 5, 1, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Liga_Equipos(nombreliga, topePartidos, activa, tipoDeporte) values ('LigaE4', 3, 1, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Liga_Equipos(nombreliga, topePartidos, activa, tipoDeporte) values ('LigaE5', 5, 1, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Liga_Equipos(nombreliga, topePartidos, activa, tipoDeporte) values ('LigaE6', 3, 1, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Liga_Equipos(nombreliga, topePartidos, activa, tipoDeporte) values ('LigaE7', 5, 1, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Liga_Equipos(nombreliga, topePartidos, activa, tipoDeporte) values ('LigaE8', 3, 1, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 1 WHERE Id = 1;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 1 WHERE Id = 2;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 1 WHERE Id = 3;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 1 WHERE Id = 4;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 1 WHERE Id = 5;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Liga_Equipos SET topePartidos = 0 WHERE Id = 1;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 2 WHERE Id = 6;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 2 WHERE Id = 7;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 2 WHERE Id = 8;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Liga_Equipos SET topePartidos = 0 WHERE Id = 2;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 3 WHERE Id = 9;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 3 WHERE Id = 10;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 3 WHERE Id = 11;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 3 WHERE Id = 12;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 3 WHERE Id = 13;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Liga_Equipos SET topePartidos = 0 WHERE Id = 3;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 4 WHERE Id = 14;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 4 WHERE Id = 15;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 4 WHERE Id = 16;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Liga_Equipos SET topePartidos = 0 WHERE Id = 4;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 5 WHERE Id = 17;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 5 WHERE Id = 18;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 5 WHERE Id = 19;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 5 WHERE Id = 20;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 5 WHERE Id = 21;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Liga_Equipos SET topePartidos = 0 WHERE Id = 5;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 6 WHERE Id = 22;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 6 WHERE Id = 23;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 6 WHERE Id = 24;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Liga_Equipos SET topePartidos = 0 WHERE Id = 6;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 7 WHERE Id = 25;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 7 WHERE Id = 26;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 7 WHERE Id = 27;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 7 WHERE Id = 28;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 7 WHERE Id = 29;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Liga_Equipos SET topePartidos = 0 WHERE Id = 7;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 8 WHERE Id = 30;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 8 WHERE Id = 31;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Partidos SET enUso = 1, Liga_EquipoId = 8 WHERE Id = 32;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "update Liga_Equipos SET topePartidos = 0 WHERE Id = 8;";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                
                cadena = "insert into Usuario(email, nombre, password, billetera, tipoRol) values ('usuario1@gmail.com', 'Usuario1', 'pass1', 10000, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Usuario(email, nombre, password, billetera, tipoRol) values ('usuario2@gmail.com', 'Usuario2', 'pass2', 10000, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Usuario(email, nombre, password, billetera, tipoRol) values ('usuario3@gmail.com', 'Usuario3', 'pass3', 10000, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Usuario(email, nombre, password, billetera, tipoRol) values ('usuario4@gmail.com', 'Usuario4', 'pass4', 10000, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Usuario(email, nombre, password, billetera, tipoRol) values ('usuario5@gmail.com', 'Usuario5', 'pass5', 10000, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Usuario(email, nombre, password, billetera, tipoRol) values ('usuario6@gmail.com', 'Usuario6', 'pass6', 10000, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Usuario(email, nombre, password, billetera, tipoRol) values ('usuario7@gmail.com', 'Usuario7', 'pass7', 10000, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Usuario(email, nombre, password, billetera, tipoRol) values ('usuario8@gmail.com', 'Usuario8', 'pass8', 10000, 2)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Administradores(email, nombre, password, billetera, tipo_Rol) values ('admin1@gmail.com', 'Admin1', 'pass1', 10000, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Administradores(email, nombre, password, billetera, tipo_Rol) values ('admin2@gmail.com', 'Admin2', 'pass2', 10000, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Administradores(email, nombre, password, billetera, tipo_Rol) values ('admin3@gmail.com', 'Admin3', 'pass3', 10000, 1)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Empresas(email, nombre, pass, billetera, tipoRol) values ('empresa1@gmail.com', 'Empresa1', 'pass1', 10000, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Empresas(email, nombre, pass, billetera, tipoRol) values ('empresa2@gmail.com', 'Empresa2', 'pass2', 10000, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Empresas(email, nombre, pass, billetera, tipoRol) values ('empresa3@gmail.com', 'Empresa3', 'pass3', 10000, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Empresas(email, nombre, pass, billetera, tipoRol) values ('empresa4@gmail.com', 'Empresa4', 'pass4', 10000, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Empresas(email, nombre, pass, billetera, tipoRol) values ('empresa5@gmail.com', 'Empresa5', 'pass5', 10000, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Empresas(email, nombre, pass, billetera, tipoRol) values ('empresa6@gmail.com', 'Empresa6', 'pass6', 10000, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Empresas(email, nombre, pass, billetera, tipoRol) values ('empresa7@gmail.com', 'Empresa7', 'pass7', 10000, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "insert into Empresas(email, nombre, pass, billetera, tipoRol) values ('empresa8@gmail.com', 'Empresa8', 'pass8', 10000, 3)";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                conexion.Close();
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message + "Error en la base de datos.");
            }
        }*/
    }
}
