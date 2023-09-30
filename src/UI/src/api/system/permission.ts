import { http } from "@/utils/http";

type Result = {
  succeeded: boolean;
  data?: object;
  errors: Array<string>;
  errorMessage: string;
  code: number;
};

type ResultArray = {
  succeeded: boolean;
  data?: Array<any>;
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

/** 获取用列表 */
export const getPermissionList = () => {
  return http.request<ResultArray>("get", "/api/Permission/Query/All");
};

/** 新增权限 */
export const addPermission = (data?: object) => {
  return http.request<Result>("post", "/api/Permission/Add", {
    data
  });
};

/** 修改权限 */
export const updatePermission = (data?: object) => {
  return http.request<Result>("put", "/api/Permission/Update", {
    data
  });
};

/** 删除权限 */
export const deletePermission = (data?: object) => {
  return http.request<Result>("delete", "/api/Permission/Delete", {
    data
  });
};

/** 同步API权限 */
export const syncAPIToPermission = () => {
  return http.request<Result>("post", "/api/Permission/Sync/API");
};
