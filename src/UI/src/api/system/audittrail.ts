
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
export const getAuditTrailList = (data?: object) => {
  return http.request<ResultTable>("post", "/api/AuditTrail/PaginationQuery", {
    data
  });
};

/** 新增 */
export const addAuditTrail = (data?: object) => {
  return http.request<Result>("post", "/api/AuditTrail/Add", {
    data
  });
};

/** 修改 */
export const updateAuditTrail = (data?: object) => {
  return http.request<Result>("put", "/api/AuditTrail/Update", {
    data
  });
};

/** 批量删除 */
export const onbatchDeleteAuditTrail = (data?: object) => {
  return http.request<Result>("delete", "/api/AuditTrail/Delete", {
    data
  });
};
