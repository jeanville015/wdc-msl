using AutoMapper;
using IDM.DTO.User;
using IDM.Model.User;
using IDM.Repository.User.Interface;
using IDM.Service.User.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Service.User.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _Mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _Mapper = mapper;
        }

        public async Task<RoleDTO> GetByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            return _Mapper.Map<RoleDTO>(role);
        }

        public async Task<int> CreateAsync(RoleDTO roleDTO)
        {
            var existingRole = await GetByRoleName(roleDTO.User_Role);
            if (existingRole != null)
            {
                return -1;
            }

            var role = _Mapper.Map<Role>(roleDTO);
            return await _roleRepository.CreateAsync(role);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _roleRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<RoleDTO>> GetAllAsync()
        {
            var role = await _roleRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<RoleDTO>>(role);
        }

        public async Task<bool> UpdateAsync(RoleDTO roleDTO)
        {
            var existingRole = await GetByRoleName(roleDTO.User_Role);

            if (existingRole != null && existingRole.Id != roleDTO.Id)
            {
                return false;
            }

            var role = _Mapper.Map<Role>(roleDTO);
            return await _roleRepository.UpdateAsync(role);
        }

        public async Task<RoleDTO> GetByRoleName(string name)
        {
            var role = await _roleRepository.GetByRoleName(name);
            return _Mapper.Map<RoleDTO>(role);
        }
    }
}
