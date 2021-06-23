using EfasisCMS.n11OrderService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;


namespace EfasisCMS.entegrasyon.pazaryeri.N11
{

    public class OrderList
    {
        clstblpazaryerleri pazaryerisinifi = new clstblpazaryerleri();
        public List<n11OrderService.OrderData> List()
        {
            DataTable dtpazaryeri = pazaryerisinifi.SatirGetirisimile("N11");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = true;
            var key = dtpazaryeri.Rows[0]["appKey"].ToString();

            var secret = dtpazaryeri.Rows[0]["appSecret"].ToString();
            String strStartDate = "";
            String strEndDate = "";
            String strOrderStatus = "1";
            String strRecipient = "";
            String strBuyerName = "";
            String strOrderNumber = "New";
            String strProductSellerCode = "";
            long productIdValue = 0;
            int currentPageValue = 1;
            int pageSizeValue = 10;

            n11OrderService.Authentication authentication = new n11OrderService.Authentication();
            authentication.appKey = key;
            authentication.appSecret = secret;

            n11OrderService.OrderSearchPeriod orderSearchPeriod = new n11OrderService.OrderSearchPeriod();
            orderSearchPeriod.startDate = strStartDate;
            orderSearchPeriod.endDate = strEndDate;


            n11OrderService.OrderDataListRequest orderDataRequest = new n11OrderService.OrderDataListRequest();
            orderDataRequest.productId = productIdValue;
            orderDataRequest.period = orderSearchPeriod;
            orderDataRequest.productSellerCode = strProductSellerCode;
            orderDataRequest.recipient = strRecipient;
            orderDataRequest.orderNumber = strOrderNumber;
            orderDataRequest.buyerName = strBuyerName;
            orderDataRequest.status = strOrderStatus;

            n11OrderService.RequestPagingData requestPagingData = new n11OrderService.RequestPagingData();
            requestPagingData.currentPage = currentPageValue;
            requestPagingData.pageSize = pageSizeValue;



            n11OrderService.OrderListRequest request = new n11OrderService.OrderListRequest();
            request.auth = authentication;
            request.pagingData = requestPagingData;
            request.searchData = orderDataRequest;

            n11OrderService.OrderServicePortClient orderServicePort = new n11OrderService.OrderServicePortClient();
            n11OrderService.OrderListResponse response = orderServicePort.OrderList(request);
            List<n11OrderService.OrderData> orderlist = new List<n11OrderService.OrderData>();
            orderlist = response.orderList.ToList();
            return orderlist.ToList();
        }


























    }

}