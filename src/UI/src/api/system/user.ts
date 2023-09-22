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

/** 获取用户管理列表 */
export const getUserList = (data?: object) => {
  return http.request<ResultTable>("post", "/api/User/PaginationQuery", {
    data
  });
};

/** 新增用户 */
export const addUser = (data?: object) => {
  return http.request<Result>("post", "/api/User/Add", {
    data
  });
};

/** 修改用户 */
export const updateUser = (data?: object) => {
  return http.request<Result>("put", "/api/User/Update", {
    data
  });
};

/** 修改用户 */
export const updateUserAvatar = (data?: object) => {
  return http.request<Result>("put", "/api/User/Update/Avatar", {
    data
  });
};

/** 批量删除用户 */
export const onbatchDeleteUser = (data?: object) => {
  return http.request<Result>("delete", "/api/User/Delete", {
    data
  });
};

/** 获取所有角色 */
export const getAllRole = () => {
  return http.request<ResultArray>("get", "/api/Role/Query/All");
};

/** 获取所有用户 */
export const getAllUser = () => {
  return http.request<ResultArray>("get", "/api/User/Query/All");
};
