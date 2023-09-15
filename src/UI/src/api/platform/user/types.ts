
export type UserAddType = {
  userName: string
  password: string
  email: string
  phoneNumber: string
}

export type UserListType = {
  userName: string
  created: string
  email: string
  phoneNumber: string
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

