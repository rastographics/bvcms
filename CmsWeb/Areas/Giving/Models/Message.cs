using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using CmsData;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Newtonsoft.Json;

namespace CmsWeb.Areas.Giving.Models
{
    public class Message : ActionResult
    {
        public int version = 0;

        public decimal totalAmountProcessed = 0;

        public int error = 1;
        public int returnCode = 0;
        public int count = 0;

        public int id = 0;

        public int argInt = 0;
        public string argString = "";
        public bool argBool = false;

        public string kiosk = "";
        public string data = "";
        public string token = "";
        public string key = "";

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
            context.HttpContext.Response.Output.Write(JsonConvert.SerializeObject(this));
        }

        public static Message createErrorReturn(string sErrorMessage, int errorCode = 1)
        {
            return new Message
            {
                data = sErrorMessage,
                error = errorCode
            };
        }

        public static Message createTypeErrorReturn()
        {
            return new Message
            {
                data = "ERROR: Type mis-match in API call."
            };
        }

        public static Message createFromString(string json)
        {
            return !string.IsNullOrEmpty(json) ? JsonConvert.DeserializeObject<Message>(json) : new Message();
        }

        public Message setData(string value)
        {
            data = value;

            return this;
        }

        public void setError(int value)
        {
            error = value;
        }

        public void setNoError()
        {
            error = API_ERROR_NONE;
        }

        public string[] getArgStringAsArray(string separator)
        {
            string[] separators = { separator };
            string[] parts = argString.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            for (int iX = 0; iX < parts.Length; iX++)
            {
                parts[iX] = parts[iX].Trim();
            }

            return parts;
        }

        public static Message successMessage(string message, int errorCode = 0)
        {
            return new Message
            {
                data = message,
                error = errorCode
            };
        }

        public static Message successMessage(string message, int errorCode = 0, decimal totalAmountProcessed = 0)
        {
            return new Message
            {
                data = message,
                error = errorCode,
                totalAmountProcessed = totalAmountProcessed
            };
        }

        public static Message errorMessage(string message, int errorCode = 0)
        {
            return new Message
            {
                data = message,
                error = errorCode
            };
        }

        public List<ScheduledGift> ScheduledGift { get; set; }
        public static Message deleteScheduleSuccess(List<ScheduledGift> list, int errorCode = 1)
        {
            return new Message
            {
                ScheduledGift = list,
                error = errorCode
            };
        }

        // API Errors
        public const int API_ERROR_NONE = 0;
        public const int API_ERROR_Database_Exception = 1;
        public const int API_ERROR_INVALID_CREDENTIALS = -6;

        public const int API_ERROR_PERSON_NOT_FOUND = 100;
        public const int API_ERROR_GIVING_PAGE_ID_NOT_FOUND = 101;

        public const int API_ERROR_PAYMENT_METHOD_NOT_FOUND = 110;
        public const int API_ERROR_PAYMENT_METHOD_IN_USE = 111;
        public const int API_ERROR_PAYMENT_METHOD_TYPE_ID_NOT_FOUND = 112;
        public const int API_ERROR_PAYMENT_METHOD_REQUIRED_FIELD_EMPTY = 113;
        public const int API_ERROR_PAYMENT_METHOD_CREDIT_CARD_NUM_INVALID = 114;
        public const int API_ERROR_PAYMENT_METHOD_CREDIT_CARD_EXPIRED = 115;
        public const int API_ERROR_PAYMENT_METHOD_BANK_ACCOUNT_NUM_INVALID = 116;
        public const int API_ERROR_PAYMENT_METHOD_BANK_ROUTING_NUM_INVALID = 117;
        public const int API_ERROR_PAYMENT_METHOD_AUTHORIZATION_FAILED = 118;

        public const int API_ERROR_SCHEDULED_GIFT_NOT_FOUND = 120;
        public const int API_ERROR_SCHEDULED_GIFT_TYPE_ID_NOT_FOUND = 121;
        public const int API_ERROR_SCHEDULED_GIFT_START_DATE_NOT_FOUND = 122;
        public const int API_ERROR_SCHEDULED_GIFT_FUND_ID_NOT_FOUND = 123;
        public const int API_ERROR_SCHEDULED_GIFT__PEOPLE_ID_NOT_FOUND = 124;

        public const int API_ERROR_SCHEDULED_GIFT_AMOUNT_NOT_FOUND = 130;

        public const int API_ERROR_CREDIT_CARD_AUTHORIZATION_FAILED = 140;
        public const int API_ERROR_CREDIT_CARD_PAYMENT_FAILED = 141;
        public const int API_ERROR_BANK_PAYMENT_FAILED = 142;

        // API Response Codes
        public const int API_OK = 200;
        public const int API_Created = 201;

        // API Giving Page Version
        // Version 1 is the initial release
        public const int API_V1 = 1;
    }
}
