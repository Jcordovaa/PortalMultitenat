using ApiPortal.Dal.Models_Portal;
using ApiPortal.Dal.Models_Admin;
using ApiPortal.Security;
using ApiPortal.Services;
using ApiPortal.ViewModelsPortal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cors;

namespace ApiPortal.Controllers
{
    //[EnableCors()]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly PortalClientesSoftlandContext _context;

        public AuthController(IConfiguration configuration, IUserService userService, PortalClientesSoftlandContext context)
        {
            _configuration = configuration;
            _userService = userService;
            _context = context;
        }

        [HttpPost("authenticate")]
        public ActionResult Authenticate(AuthenticateVm model)
        {
            try
            {
                
                HashPassword aux = new HashPassword();
                string hashPassword = aux.HashCode(model.Password);
                bool isCredentialValid = false;
                string fullName = string.Empty;
                string email = string.Empty;
                string rut = string.Empty;
                string codAux = string.Empty;
                Boolean esUsuario = false;

                var configuracionEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();

                if (model.Rut == configuracionEmpresa.RutEmpresa)//Login usuario administrador Softland
                {
                    var usuario = _context.Usuarios.Where(x => x.Email == model.Email && x.Password == hashPassword).FirstOrDefault();

                    if (usuario == null)
                        return BadRequest();

                    if (usuario.CuentaActivada == null || usuario.CuentaActivada == 0)
                        return BadRequest("Cuenta ingresada aún no ha sido activada por el usuario.");

                    fullName = $"{usuario.Nombres} {usuario.Apellidos}";
                    isCredentialValid = true;
                    email = usuario.Email;
                    esUsuario = true;
                }
                else //Cliente
                {

                    var cliente = new ClientesPortal();
                    if (string.IsNullOrEmpty(model.CodAux))
                    {
                        //Solo un registro por rut de empresa
                        cliente = _context.ClientesPortals.Where(x => x.Rut == model.Rut && x.Correo == model.Email && x.Clave == hashPassword).FirstOrDefault();
                    }
                    else
                    {
                        //Mas de un registro por rut de empresa
                        cliente = _context.ClientesPortals.Where(x => x.Rut == model.Rut && x.CodAux == model.CodAux && x.Correo == model.Email && x.Clave == hashPassword).FirstOrDefault();
                    }


                    if (cliente == null)
                        return BadRequest();

                    fullName = $"{cliente.Nombre}";
                    email = cliente.Correo;
                    rut = cliente.Rut;
                    codAux = cliente.CodAux;
                    isCredentialValid = true;
                }

                if (isCredentialValid)
                {
                    var token = this.CrearToken(model.Email);
                    return Ok(new
                    {
                        Email = email,
                        Rut = rut,
                        CodAux = codAux,
                        Token = token.Token,
                        Nombre = fullName,
                        EsUsuario = esUsuario,
                    });
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception e)
            {

                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "api/auth/authenticate"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest();
            }
        }

        private TokenUsuario CrearToken(string email)
        {


            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, RolesUsuario.Cliente)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var fechaCreacion = DateTime.Now;
            var fechaExpiracion = fechaCreacion.AddDays(1);

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: fechaExpiracion,
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            TokenUsuario tokenUsuario = new TokenUsuario
            {
                Token = jwt,
                TokenCreated = fechaCreacion,
                TokenExpires = fechaExpiracion

            };

            return tokenUsuario;
        }
    }
}
