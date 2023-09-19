export interface DepartmentItem {
  id: string
  departmentName: string
  children?: DepartmentItem[]
}

export interface DepartmentListResponse {
  list: DepartmentItem[]
}

export interface DepartmentUserParams {
  pageSize: number
  pageIndex: number
  id: string
  username?: string
  account?: string
}

export interface DepartmentUserItem {
  id: string
  username: string
  account: string
  email: string
  createTime: string
  role: string
  department: DepartmentItem
}

export interface DepartmentUserResponse {
  list: DepartmentUserItem[]
  total: number
}


export type UserQueryType = {
  keyword: string,
  pageNumber: Number,
  pageSize: Number,
  orderBy: string,
  sortDirection: string,
  userName: string,
  email: string,
  emailConfirmed: boolean,
  phoneNumber: string,
  lockoutEnabled: boolean
}

// 响应接口
export interface UserPaginationQueryResponse {
  /*是否成功 */
  succeeded: boolean;

  /*错误信息数组 */
  errors: Record<string, unknown>[];

  /*错误信息 */
  errorMessage: string;

  /*状态码 0成功 1失败 */
  code: number;

  /* */
  data: {
    /*当前页 */
    currentPage: number;

    /*总条数 */
    totalItems: number;

    /*总页数 */
    totalPages: number;

    /*有上一页 */
    hasPreviousPage: boolean;

    /*有下一页 */
    hasNextPage: boolean;

    /*返回数据 */
    items: {
      /*唯一标识 */
      id: number;

      /*用户唯一标识 */
      userId: number;

      /*用户名 */
      userName: string;

      /*标准化用户名 */
      normalizedUserName: string;

      /*邮箱 */
      email: string;

      /*标准化邮箱 */
      normalizedEmail: string;

      /*确认邮箱 */
      emailConfirmed: boolean;

      /*安全印章（修改敏感信息时必须修改） */
      securityStamp: string;

      /*并发标记 */
      concurrencyStamp: string;

      /*手机号码 */
      phoneNumber: string;

      /*确认手机号码 */
      phoneNumberConfirmed: boolean;

      /*启用双因子 */
      twoFactorEnabled: boolean;

      /*锁定结束时间 */
      lockoutEnd: Record<string, unknown>;

      /*已锁定 */
      lockoutEnabled: boolean;

      /*访问失败次数 */
      accessFailedCount: number;

      /*头像图片 */
      profilePictureDataUrl: string;

      /*是否激活 */
      isActive: boolean;

      /*是否活跃 */
      isLive: boolean;

      /*创建时间 */
      created: Record<string, unknown>;
    }[];
  };
}
