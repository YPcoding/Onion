
import { http } from "@/utils/http";

type Result = {
  succeeded: boolean;
  data?: object;
  errors: Array<string>;
  errorMessage: string;
  code: number;
};

type ResultTable = {
  succeeded: boolean;
  errors: Array<any>;
  errorMessage: string;
  code: number;
  data?: {
    /** 总条目数 */
    totalItems?: number;
    /** 当前页数 */
    currentPage?: number;
    /** 每页大小 */
    pageSize?: number;
    /** 总页数 */
    totalPages?: number;
    /** 有上一页 */
    hasPreviousPage?: boolean;
    /** 有下一页 */
    hasNextPage?: boolean;
    /** 列表数据 */
    items: Array<any>;
  };
};

/** 分页查询 */
export const getNotificationList = (data?: object) => {
  return http.request<ResultTable>("post", "/api/Notification/PaginationQuery", {
    data
  });
};

/** 新增 */
export const addNotification = (data?: object) => {
  return http.request<Result>("post", "/api/Notification/Add", {
    data
  });
};

/** 修改 */
export const updateNotification = (data?: object) => {
  return http.request<Result>("put", "/api/Notification/Update", {
    data
  });
};

/** 批量删除 */
export const onbatchDeleteNotification = (data?: object) => {
  return http.request<Result>("delete", "/api/Notification/Delete", {
    data
  });
};
