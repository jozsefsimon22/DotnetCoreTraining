using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;
using System;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class StocksService : IStocksService
    {
        //private field
        private readonly OrdersDbContext _db;



        /// <summary>
        /// Constructor of StocksService class that executes when a new object is created for the class
        /// </summary>
        public StocksService(OrdersDbContext db)
        {
            _db = db;
        }


        public async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? buyOrderRequest)
        {
            //Validation: buyOrderRequest can't be null
            if (buyOrderRequest == null)
                throw new ArgumentNullException(nameof(buyOrderRequest));

            //Model validation
            ValidationHelper.ModelValidation(buyOrderRequest);

            //convert buyOrderRequest into BuyOrder type
            BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();

            //generate BuyOrderID
            buyOrder.BuyOrderID = Guid.NewGuid();

            //add buy order object to buy orders list
            await _db.BuyOrders.AddAsync(buyOrder);
            await _db.SaveChangesAsync();

            //convert the BuyOrder object into BuyOrderResponse type
            return buyOrder.ToBuyOrderResponse();
        }


        public async Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest)
        {
            //Validation: sellOrderRequest can't be null
            if (sellOrderRequest == null)
                throw new ArgumentNullException(nameof(sellOrderRequest));

            //Model validation
            ValidationHelper.ModelValidation(sellOrderRequest);

            //convert sellOrderRequest into SellOrder type
            SellOrder sellOrder = sellOrderRequest.ToSellOrder();

            //generate SellOrderID
            sellOrder.SellOrderID = Guid.NewGuid();

            //add sell order object to sell orders list
            await _db.SellOrders.AddAsync(sellOrder);
            await _db.SaveChangesAsync();

            //convert the SellOrder object into SellOrderResponse type
            return sellOrder.ToSellOrderResponse();
        }


        public async Task<List<BuyOrderResponse>> GetBuyOrders()
        {
            //Convert all BuyOrder objects into BuyOrderResponse objects
            return await _db.BuyOrders.OrderByDescending(temp => temp.DateAndTimeOfOrder)
             .Select(temp => temp.ToBuyOrderResponse()).ToListAsync();
        }


        public async Task<List<SellOrderResponse>> GetSellOrders()
        {
            //Convert all SellOrder objects into SellOrderResponse objects
            return await _db.SellOrders
             .OrderByDescending(temp => temp.DateAndTimeOfOrder)
             .Select(temp => temp.ToSellOrderResponse()).ToListAsync();
        }
    }
}


