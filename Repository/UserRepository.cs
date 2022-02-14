using APIClientes.Data;
using APIClientes.Models;
using APIClientes.Models.Dto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APIClientes.Repository
{
    // HACEMOS EXTENDER LA CLASE DE IUSERREPOSITORY
    public class UserRepository : IUsersRepository
    {
        // CREAMOS LAS VARIABLES PARA LA CONEXION CON BD Y EL MAPEO Y LAS INICIALIZAMOS EN EL CONTRUCTOR
        private readonly ApplicationDbContext _dbContext;

        // VAMOS A DECLARAR UN ICONFIGURATION Y LO INICIALIZAMOS EN EL CONSTRUCTOR
        private readonly IConfiguration _configuration;// TRABAJAR CON TOKENS

        private IMapper _mapper;
        

        public UserRepository(ApplicationDbContext dbContext, IConfiguration configuarton, IMapper mapper)
        {
            _dbContext = dbContext;
            _configuration = configuarton;
            _mapper = mapper;
        }
        public async Task<bool> GetUser(string userName)
        {
            // VERIFICAR SI EL USUARIO EXISTE
            if (await _dbContext.Users.AnyAsync(x => x.UserName.ToLower().Equals(userName.ToLower())))
            {
                return true;

            }
            return false;
        }

        public async Task<string> Login(string userName, string password)
        {
            // VERIFICAMOS SI EL USUARIO EXISTE
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName.ToLower().Equals(userName.ToLower()));

            if (user == null)
            {
                return "NoUser";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) // VERIFICAMOS QUE SEA ESA SU CONTRASEÑA
            {
                return "WrongPassword";
            }
            else
            {
                return CreateToken(user);
            }
        }

        public async Task<int> RegisterUser(User user, string password)
        {
            // UTILIZAMOS UN TRY CATCH
            try
            {
                // VERIFICAMOS SI EL USUARIO EXITE
                if (await GetUser(user.UserName))
                {
                    return -1;
                }
                // ENCRIPTAMOS LA CLAVE CON LA NUEVA FUNCION VOID QUE CREAMOS
                CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                // CAMBIAMOS LOS VALORES DEL passwordHash y passwordSalt con los que retorna la funcion
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                // GRABAMOS LOS CAMBIOS
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                return user.Id;

            }
            catch (System.Exception)
            {

                return -500;
            }
        }

        // METODO PARA ENCRIPTAR PASSWORD
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;

                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));


            }
        }

        // METODO PARA VERIFICAR LAS CLAVES ENCRIPTADAS
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt) ) 
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                // HAREMOS UN FOR PARA CONPROBAR CARACTER POR CARACTER SI COINCIDE NUESTRO PASSWORD
                for (int i = 0; i < computedHash.Length; i++)
                {
                    // VERIFICAMOS CARACTER POR CARACTER
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        // METODO PARA CREAR UN NUEVO TOKEN
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                // ENVIAMOS EL ID Y EL USERNAME PARA QUE SEAN PARTE DE TODO EL TOKEN
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            // AQUI LO QUE SE HACE ES QUE TODA LA CLAVE SECRETA + ID + USERNAME TODO ESTO VA A SER PARTE DEL TOKEN
            // PARA QUE EL USUARIO ESTE AUTORIZADO
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.
                                        GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            // SE CREA UNA VARIABLE CREDS QUE VA A VALIDAR LAS CREDENCIALES
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // SE CREA UNA VARIABLE TOKEN
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.Now.AddDays(1),// Fecha de expiracion un dia
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);



        }

        public async Task<UserDto> GetUserById(int id)
        {
            // OBTENEMOS EL USER POR EL ID
            User user = await _dbContext.Users.FindAsync(id);

            // HACERMOS UN MAPEO PARA RETORNAR EL USERDTO pasando el OBJ user
            return _mapper.Map<UserDto>(user);
        }

        public async Task<List<UserDto>> GetUsers()
        {
            // OBTENEMOS LA LISTA DE USERS
            List<User> listaUsers = await _dbContext.Users.ToListAsync();

            // HACEMOS EL MAPEO PARA RETORNAR DE TIPO USERDTO pasando la VARIABLE listaUsers
            return _mapper.Map<List<UserDto>>(listaUsers);
        }

        public async Task<bool> DeleteUser(int id)
        {
            // HACEMOS EL CONTROL A TRAVES DE UN TRY CATCH
            try
            {
                // VERIFICAR SI EL REGISTRO EXISTE
                User user = await _dbContext.Users.FindAsync(id);
                if (user == null)
                {
                    return false;
                }
                // SI EXISTE BORRAMOS EL USER
                _dbContext.Users.Remove(user);
                // GRABAMOS TODOS LOS CAMBIOS
                await _dbContext.SaveChangesAsync();

                return true;

            }
            catch (System.Exception)
            {

                return false;
            }
        }

        /*public async Task<UserDto> CreateUpdate(User user, string password)
        {
            // VAMOS A MAPEAR PORQUE ESTAMOS RECIBIENDO UN USERDTO para llevarlo a USER
            User user = _mapper.Map<UserDto, User>(userDto);

            // VERIFICAMOS SI ES UNA ACTUALIZACION
            if (user.Id > 0) // SI ES MAYOR QUE CERO ES UNA ACTUALIZACION
            {
                // ENVIAMOS TODO EL OBJETO CLIENTE
                _dbContext.Users.Update(user);
            }
            else
            {
                // EN CASO CONTRARIO SE TRATA DE UNA CREACION DE UN NUEVO REGISTRO
                await _dbContext.Users.AddAsync(user);
            }
            // GRABAMOS LOS CAMBIOS
            await _dbContext.SaveChangesAsync();

            // HACEMOS EL RETONO DE TIPO CLIENTEDTO MAPENADO
            return _mapper.Map<User, UserDto>(user);
        }*/
    }
}
