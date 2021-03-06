using APIClientes.Models;
using APIClientes.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIClientes.Repository
{
    public interface IUsersRepository
    {
        // AQUI EN LA INTERFAZ ESTAN TODOS LOS METODOS QUE VAMOS A CREAR PARA TRABAJAR USUARIOS EN EL REPOSITORIO
        Task<int> RegisterUser(User user, string password);
        Task<string> Login(string userName, string password);
        Task<bool> GetUser(string userName);
        Task<UserDto> GetUserById(int id);
        Task<List<UserDto>> GetUsers();
        Task<bool> DeleteUser(int id);
        //Task<UserDto> CreateUpdate(UserDto userDto);
    }
}
