using APIClientes.Models;
using APIClientes.Models.Dto;
using APIClientes.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIClientes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase

    {   
        // HACEMOS UNA INSTANCIA DE NUESTRA INTERFAZ PARA TRAER LOS METODOS DESDE EL REPOSITORIO
        private readonly IUsersRepository _userRepository;

        // HACEMOS UN OBJETO DE RESPONSEDTO Y LO INICIALIZAMOS EN EL CONSTRUCTOR
        protected ResponseDto _responseDto;

        public UsersController(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
            _responseDto = new ResponseDto();
        }

        // CREAMOS UN NUEVO METODO DE TIPO POST Y LE PASAMOS EL NOMBRE DE LA RUTA EN ESTE CASO Register
        [HttpPost("Register")]
        public async Task<ActionResult> Register(UserDto user)
        {
            var response = await _userRepository.RegisterUser(new User
            {
                UserName = user.UserName

            }, user.Password);

            //SE VALIDA SI ES QUE EL USUARIO YA EXISTE
            if (response == -1)
            {
                _responseDto.IsSuccess = false;
                _responseDto.DisplayMessage = "El Usuario ya existe";
                return BadRequest(_responseDto);
            }

            // SE VALIDA SI HUBO UN PROBLEMA CON LA CREACION DEL USUARIO
            if(response == -500)
            {
                _responseDto.IsSuccess = false;
                _responseDto.DisplayMessage = "Error al crear el Usuario";
                return BadRequest(_responseDto);
            }

            // SI EL USUARIO ES CREADO SATISFACTORIAMENTE
            _responseDto.DisplayMessage = "El Usuario fue creado con éxito";
            _responseDto.Result = response;
            return Ok(_responseDto);
        }

        // CREAMOS UN NUEVO METODO DE TIPO POST Y LE PASAMOS EL NOMBRE DE LA RUTA EN ESTE CASO Login
        [HttpPost("Login")]
        public async Task<ActionResult> Login(UserDto user)
        {
            var response = await _userRepository.Login(user.UserName, user.Password);

            if (response == "NoUser")
            {
                _responseDto.IsSuccess = false;
                _responseDto.DisplayMessage = "El Usuario no existe";
                return BadRequest(_responseDto);
            }
            else if (response == "WrongPassword")
            {
                _responseDto.IsSuccess = false;
                _responseDto.DisplayMessage = "la Contraseña es incorrecta";
                return BadRequest(_responseDto);
            }
            else
            {
                _responseDto.Result = response;
                _responseDto.DisplayMessage = "Usuario Conectado";
                return Ok(_responseDto);
            }            
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var listaUsers = await _userRepository.GetUsers();
                _responseDto.Result = listaUsers;
                _responseDto.DisplayMessage = "Lista de Users";

            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string> { ex.ToString() };
            }

            return Ok(_responseDto);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetUser(int id)
        {
            // VALIDO SI EL ID EXISTE
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.DisplayMessage = "User No Existe";
                return NotFound(_responseDto);
            }

            _responseDto.Result = user;
            _responseDto.DisplayMessage = "Información del User";
            return Ok(_responseDto);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // UTILIZAMOS TRY CATCH
            try
            {
                bool userEliminado = await _userRepository.DeleteUser(id);
                if (userEliminado)
                {
                    _responseDto.Result = userEliminado;
                    _responseDto.DisplayMessage = "User Eliminado con Exito";
                    return Ok(_responseDto);
                }
                else
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.DisplayMessage = "Error al Eliminar el User";
                    return BadRequest(_responseDto);
                }
            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string> { ex.ToString() };
                return BadRequest(_responseDto);
            }
        }
    }
}
