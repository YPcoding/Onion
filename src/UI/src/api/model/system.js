import config from "@/config"
import http from "@/utils/request"

export default {
	menu: {
		myMenus: {
			url: `${config.API_URL}/Auth/Loginer/Muens`,
			name: "获取我的菜单",
			get: async function () {
				return await http.get(this.url)
			}
		},
		list: {
			url: `${config.API_URL}/Permission/Query/All`,
			name: "获取菜单",
			get: async function () {
				return await http.get(this.url)
			}
		},
		roleMenus: {
			url: `${config.API_URL}/Role/Query/Permission/By`,
			name: "获取角色菜单",
			get: async function (roleId) {
				return await http.get(`${this.url}/${roleId}`)
			}
		},
		permissionMenus: {
			url: `${config.API_URL}/Role/Permission/Menu`,
			name: "修改角色菜单权限",
			put: async function (params) {
				return await http.put(this.url, params)
			}
		}
	},
	dic: {
		tree: {
			url: `${config.API_URL}/system/dic/tree`,
			name: "获取字典树",
			get: async function () {
				return await http.get(this.url)
			}
		},
		list: {
			url: `${config.API_URL}/system/dic/list`,
			name: "字典明细",
			get: async function (params) {
				return await http.get(this.url, params)
			}
		},
		get: {
			url: `${config.API_URL}/system/dic/get`,
			name: "获取字典数据",
			get: async function (params) {
				return await http.get(this.url, params)
			}
		}
	},
	role: {
		list: {
			url: `${config.API_URL}/Role/PaginationQuery`,
			name: "获取角色列表",
			post: async function (params) {
				return await http.post(this.url, params)
			}
		},
		all: {
			url: `${config.API_URL}/Role/Query/All`,
			name: "获取所有角色",
			get: async function () {
				return await http.get(this.url)
			}
		},
		add: {
			url: `${config.API_URL}/Role/Add`,
			name: "新增角色",
			post: async function (params) {
				return await http.post(this.url, params)
			}
		},
		update: {
			url: `${config.API_URL}/Role/Update`,
			name: "修改角色",
			put: async function (params) {
				return await http.put(this.url, params)
			}
		},
		isActive: {
			url: `${config.API_URL}/Role/IsActive`,
			name: "修改角色是否激活",
			put: async function (params) {
				return await http.put(this.url, params)
			}
		},
		delete: {
			url: `${config.API_URL}/Role/Delete`,
			name: "批量删除角色",
			delete: async function (params) {
				return await http.delete(this.url, params)
			}
		}
	},
	dept: {
		list: {
			url: `${config.API_URL}/Department/Query/All`,
			name: "获取部门列表",
			post: async function (params) {
				return await http.post(this.url, params)
			}
		},
		add: {
			url: `${config.API_URL}/Department/Add`,
			name: "新增部门",
			post: async function (params) {
				return await http.post(this.url, params)
			}
		},
		update: {
			url: `${config.API_URL}/Department/Update`,
			name: "修改部门",
			put: async function (params) {
				return await http.put(this.url, params)
			}
		},
		delete: {
			url: `${config.API_URL}/Department/delete`,
			name: "删除部门",
			delete: async function (params) {
				return await http.delete(this.url, params)
			}
		}
	},
	user: {
		list: {
			url: `${config.API_URL}/User/PaginationQuery`,
			name: "获取用户列表",
			post: async function (params) {
				return await http.post(this.url, params)
			}
		},
		info: {
			url: `${config.API_URL}/User/Info/Query`,
			name: "获取用户个人信息",
			get: async function () {
				return await http.get(this.url)
			}
		},
		updateInfo: {
			url: `${config.API_URL}/User/Info`,
			name: "修改个人信息",
			put: async function (params) {
				return await http.put(this.url, params)
			}
		},
		changePassword: {
			url: `${config.API_URL}/User/Change/Password`,
			name: "更改个人密码",
			put: async function (params) {
				return await http.put(this.url, params)
			}
		},
		saveUserProfileSettings: {
			url: `${config.API_URL}/UserProfileSetting/Save`,
			name: "个人设置",
			post: async function (name, val, type, defaultValue) {
				return await http.post(this.url, { settingName: name, settingValue: val, valueType: type, defaultValue: defaultValue })
			}
		},
		getUserProfileSettings: {
			url: `${config.API_URL}/UserProfileSetting/Query`,
			name: "个人设置",
			get: async function () {
				return await http.get(this.url)
			}
		},
		add: {
			url: `${config.API_URL}/User/Add`,
			name: "新增用户",
			post: async function (params) {
				return await http.post(this.url, params)
			}
		},
		update: {
			url: `${config.API_URL}/User/Update`,
			name: "修改用户",
			put: async function (params) {
				return await http.put(this.url, params)
			}
		},
		delete: {
			url: `${config.API_URL}/User/Delete`,
			name: "删除用户",
			delete: async function (params) {
				return await http.delete(this.url, params)
			}
		}
	},
	app: {
		list: {
			url: `${config.API_URL}/system/app/list`,
			name: "应用列表",
			get: async function () {
				return await http.get(this.url)
			}
		}
	},
	log: {
		list: {
			url: `${config.API_URL}/system/log/list`,
			name: "日志列表",
			get: async function (params) {
				return await http.get(this.url, params)
			}
		}
	},
	table: {
		list: {
			url: `${config.API_URL}/system/table/list`,
			name: "表格列管理列表",
			get: async function (params) {
				return await http.get(this.url, params)
			}
		},
		info: {
			url: `${config.API_URL}/system/table/info`,
			name: "表格列管理详情",
			get: async function (params) {
				return await http.get(this.url, params)
			}
		}
	},
	tasks: {
		list: {
			url: `${config.API_URL}/system/tasks/list`,
			name: "系统任务管理",
			get: async function (params) {
				return await http.get(this.url, params)
			}
		}
	}
}
