using APIClientes.Models;
using APIClientes.Models.Dto;
using APIClientes.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
