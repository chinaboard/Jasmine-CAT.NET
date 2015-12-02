using System;

namespace CAT
{
    public class JasmineConstants
    {
        public const int MAX_LENGTH = 1000;

        public const int MAX_ITEM_LENGTH = 50;

        /**
         * Cat instrument attribute names
         */
        public const String CAT_STATE = "cat-state";

        public const String CAT_PAGE_URI = "cat-page-uri";

        public const String CAT_PAGE_TYPE = "cat-page-type";

        /**
         * Pigeon Transation Type
         */
        public const String TYPE_PIGEONCALL = "PigeonCall";

        public const String TYPE_PIGEONCALL_APP = "PigeonCall.app";

        public const String TYPE_PIGEONCALL_SERVERIP = "PigeonCall.server";

        public const String TYPE_PIGEONCALL_PORT = "PigeonCall.port";

        public const String TYPE_CALL = "Call";

        public const String TYPE_RESULT = "Result";

        public const String TYPE_TimeOut = "PigeonTimeOut";

        public const String TYPE_PIGEONSERVICE = "PigeonService";

        public const String TYPE_PIGEONSERVICE_CLIENTIP = "PigeonService.client";

        public const String TYPE_PIGEONSERVICE_CLIENT_DOMAIN = "PigeonService.app";

        public const String TYPE_SERVICE = "Service";

        public const String TYPE_REMOTE_CALL = "RemoteCall";

        public const String TYPE_REQUEST = "Request";

        public const String TYPE_RESPONSE = "Respone";

        /**
         *  Error Type
         */

        public const String TYPE_ERROR = "Error";

        public const String TYPE_EXCEPTION = "Exception";

        public const String TYPE_RUNTIMEEXCEPTION = "RuntimeException";

        /**
         * Pigeon Event Type, it is used to record the param
         */

        public const String TYPE_PIGEON_REQUEST = "PigeonRequest";

        public const String TYPE_PIGEON_RESPONSE = "PigeonRespone";

        /**
         * Pigeon Event name
         */
        public const String NAME_REQUEST = "PigeonRequest";

        public const String NAME_RESPONSE = "PigeonRespone";

        public const String NAME_TIME_OUT = "ClientTimeOut";

        public const String NAME_Cache = "Cache";

        /**
         * Pigeon Context Info
         */
        public const String PIGEON_ROOT_MESSAGE_ID = "RootMessageId";

        public const String PIGEON_PARENT_MESSAGE_ID = "ParentMessageId";

        public const String PIGEON_CHILD_MESSAGE_ID = "ChildMessageId";

        public const String PIGEON_CURRENT_MESSAGE_ID = "CurrentMessageId";

        public const String PIGEON_SERVER_MESSAGE_ID = "ServerMessageId";

        public const String PIGEON_RESPONSE_MESSAGE_ID = "ResponseMessageId";

        public const String PIGEON_CALL_NAME = "PigeonCall.Name";

        public const String TYPE_SQL = "SQL";

        public const String TYPE_SQL_PARAM = "SQL.PARAM";

        public const String TYPE_SQL_METHOD = "SQL.Method";

        public const String TYPE_SQL_DATABASE = "SQL.Database";

        public const String TYPE_URL = "URL";

        public const String TYPE_URL_FORWARD = "URL.Forward";

        public const String TYPE_URL_SERVER = "URL.Server";

        public const String TYPE_URL_METHOD = "URL.Method";

        public const String TYPE_ACTION = "Action";

        public const String TYPE_METRIC = "MetricType";

        public const String TYPE_TRACE = "TraceMode";

        public const int ERROR_COUNT = 100;

        public const int SUCCESS_COUNT = 1000;

        public const string ContextKey = "CAT.JasmineKey";
    }
}