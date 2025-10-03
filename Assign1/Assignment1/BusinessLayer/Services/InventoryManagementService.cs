using DataAccessLayer.Repository;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class InventoryManagementService : IInventoryManagementService
    {
        private readonly IInventoryManagementRepository _inventoryRepository;
        private readonly IEVMRepository _evmRepository;

        public InventoryManagementService(
            IInventoryManagementRepository inventoryRepository,
            IEVMRepository evmRepository)
        {
            _inventoryRepository = inventoryRepository;
            _evmRepository = evmRepository;
        }

        public async Task<List<InventoryAllocation>> GetAllInventoryAllocationsAsync()
        {
            return await _inventoryRepository.GetAllInventoryAllocationsAsync();
        }

        public async Task<List<InventoryAllocation>> GetInventoryAllocationsByDealerAsync(Guid dealerId)
        {
            return await _inventoryRepository.GetInventoryAllocationsByDealerAsync(dealerId);
        }

        public async Task<List<InventoryAllocation>> GetInventoryAllocationsByProductAsync(Guid productId)
        {
            return await _inventoryRepository.GetInventoryAllocationsByProductAsync(productId);
        }

        public async Task<InventoryAllocation> GetInventoryAllocationAsync(Guid productId, Guid dealerId)
        {
            return await _inventoryRepository.GetInventoryAllocationAsync(productId, dealerId);
        }

        public async Task<bool> CreateInventoryAllocationAsync(InventoryAllocation allocation)
        {
            if (allocation.AllocatedQuantity < 0 || allocation.AvailableQuantity < 0)
                return false;

            if (allocation.MinimumStock < 0 || allocation.MaximumStock <= allocation.MinimumStock)
                return false;

            return await _inventoryRepository.CreateInventoryAllocationAsync(allocation);
        }

        public async Task<bool> UpdateInventoryAllocationAsync(InventoryAllocation allocation)
        {
            if (allocation.AllocatedQuantity < 0 || allocation.AvailableQuantity < 0)
                return false;

            if (allocation.MinimumStock < 0 || allocation.MaximumStock <= allocation.MinimumStock)
                return false;

            return await _inventoryRepository.UpdateInventoryAllocationAsync(allocation);
        }

        public async Task<bool> DeleteInventoryAllocationAsync(Guid id)
        {
            return await _inventoryRepository.DeleteInventoryAllocationAsync(id);
        }

        public async Task<List<InventoryAllocation>> GetLowStockAllocationsAsync()
        {
            return await _inventoryRepository.GetLowStockAllocationsAsync();
        }

        public async Task<List<InventoryAllocation>> GetCriticalStockAllocationsAsync()
        {
            return await _inventoryRepository.GetCriticalStockAllocationsAsync();
        }

        public async Task<List<InventoryAllocation>> GetOutOfStockAllocationsAsync()
        {
            return await _inventoryRepository.GetOutOfStockAllocationsAsync();
        }

        public async Task<List<InventoryTransaction>> GetInventoryTransactionsAsync(Guid? productId = null, Guid? dealerId = null, string transactionType = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            return await _inventoryRepository.GetInventoryTransactionsAsync(productId, dealerId, transactionType, fromDate, toDate);
        }

        public async Task<bool> CreateInventoryTransactionAsync(InventoryTransaction transaction)
        {
            return await _inventoryRepository.CreateInventoryTransactionAsync(transaction);
        }

        public async Task<bool> TransferStockAsync(Guid productId, Guid fromDealerId, Guid toDealerId, int quantity, string reason, Guid processedByUserId)
        {
            if (quantity <= 0) return false;
            if (fromDealerId == toDealerId) return false;

            return await _inventoryRepository.TransferStockAsync(productId, fromDealerId, toDealerId, quantity, reason, processedByUserId);
        }

        public async Task<bool> AdjustStockAsync(Guid productId, Guid dealerId, int quantity, string reason, Guid processedByUserId)
        {
            if (quantity == 0) return false;

            return await _inventoryRepository.AdjustStockAsync(productId, dealerId, quantity, reason, processedByUserId);
        }

        public async Task<List<InventoryAllocation>> GetInventoryReportAsync(Guid? dealerId = null, Guid? productId = null, string status = null)
        {
            return await _inventoryRepository.GetInventoryReportAsync(dealerId, productId, status);
        }

        public async Task<Dictionary<string, int>> GetStockSummaryAsync()
        {
            return await _inventoryRepository.GetStockSummaryAsync();
        }

        public async Task<List<InventoryTransaction>> GetStockMovementReportAsync(DateTime fromDate, DateTime toDate, Guid? productId = null, Guid? dealerId = null)
        {
            return await _inventoryRepository.GetStockMovementReportAsync(fromDate, toDate, productId, dealerId);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _evmRepository.GetAllProductsAsync();
        }

        public async Task<List<Dealer>> GetAllDealersAsync()
        {
            return await _evmRepository.GetAllDealersAsync();
        }
    }
}
