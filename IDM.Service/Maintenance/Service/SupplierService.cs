using AutoMapper;
using IDM.DTO.Maintenance;
using IDM.Model.Maintenance;
using IDM.Repository.Maintenance.Interface;
using IDM.Service.Maintenance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Service
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _Mapper;

        public SupplierService(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _Mapper = mapper;
        }

        public async Task<SupplierDTO> GetByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            return _Mapper.Map<SupplierDTO>(supplier);
        }

        public async Task<int> CreateAsync(SupplierDTO supplierDTO)
        {
            var existingSupplier = await GetBySupplierName(supplierDTO.Supplier_Name);
            if (existingSupplier != null)
            {
                return -1;
            }

            var supplier = _Mapper.Map<Supplier>(supplierDTO);
            return await _supplierRepository.CreateAsync(supplier);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _supplierRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<SupplierDTO>> GetAllAsync()
        {
            var supplier = await _supplierRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<SupplierDTO>>(supplier);
        }

        public async Task<bool> UpdateAsync(SupplierDTO supplierDTO)
        {
            var existingSupplier = await GetBySupplierName(supplierDTO.Supplier_Name);

            if (existingSupplier != null && existingSupplier.Id != supplierDTO.Id)
            {
                return false;
            }

            var supplier = _Mapper.Map<Supplier>(supplierDTO);
            return await _supplierRepository.UpdateAsync(supplier);
        }

        public async Task<SupplierDTO> GetBySupplierName(string name)
        {
            var supplier = await _supplierRepository.GetBySupplierName(name);
            return _Mapper.Map<SupplierDTO>(supplier);
        }
    }
}
