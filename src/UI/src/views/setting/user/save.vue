<template>
	<el-dialog :title="titleMap[mode]" v-model="visible" :width="500" destroy-on-close @closed="$emit('closed')">
		<el-form :model="form" :rules="rules" :disabled="mode=='show'" ref="dialogForm" label-width="100px" label-position="left">
			<el-form-item label="头像" prop="profilePictureDataUrl">
				<sc-upload v-model="form.profilePictureDataUrl" title="上传头像"></sc-upload>
			</el-form-item>
			<el-form-item label="登录账号" prop="userName" v-if="mode=='add'">
				<el-input v-model="form.userName" placeholder="用于登录系统" clearable></el-input>
			</el-form-item>
			<el-form-item label="姓名" prop="realname">
				<el-input v-model="form.realname" placeholder="请输入完整的真实姓名" clearable></el-input>
			</el-form-item>
			<template v-if="mode=='add'">
				<el-form-item label="登录密码" prop="password">
					<el-input type="password" v-model="form.password" clearable show-password></el-input>
				</el-form-item>
				<el-form-item label="确认密码" prop="confirmPassword">
					<el-input type="password" v-model="form.confirmPassword" clearable show-password></el-input>
				</el-form-item>
			</template>
			<el-form-item label="所属部门" prop="departmentId">
				<el-cascader v-model="form.departmentId" :options="depts" :props="deptsProps" :show-all-levels="true" clearable style="width: 100%;"></el-cascader>
			</el-form-item>
			<el-form-item label="所属角色" prop="roleIds">
				<el-select v-model="form.roleIds" multiple filterable style="width: 100%">
					<el-option v-for="item in groups" :key="item.roleId" :label="item.roleName" :value="item.roleId"/>
				</el-select>
			</el-form-item>
		</el-form>
		<template #footer>
			<el-button @click="visible=false" >取 消</el-button>
			<el-button v-if="mode!='show'" type="primary" :loading="isSaveing" @click="submit()">保 存</el-button>
		</template>
	</el-dialog>
</template>

<script>
	export default {
		emits: ['success', 'closed'],
		data() {
			return {
				mode: "add",
				titleMap: {
					add: '新增用户',
					edit: '编辑用户',
					show: '查看'
				},
				visible: false,
				isSaveing: false,
				//表单数据
				form: {
					userId:"",
					userName: "",
					realname:"",
					password: "",
					confirmPassword:"",
					roleIds:[],
					superiorId:null,
					departmentId:null,
					departmentIds:null,
					profilePictureDataUrl:""
				},
				//验证规则
				rules: {
					avatar:[
						{required: true, message: '请上传头像'}
					],
					userName: [
						{required: true, message: '请输入登录账号'}
					],
					name: [
						{required: true, message: '请输入真实姓名'}
					],
					password: [
						{required: true, message: '请输入登录密码'},
						{validator: (rule, value, callback) => {
							if (this.form.confirmPassword !== '') {
								this.$refs.dialogForm.validateField('confirmPassword');
							}
							callback();
						}}
					],
					confirmPassword: [
						{required: true, message: '请再次输入密码'},
						{validator: (rule, value, callback) => {
							if (value !== this.form.password) {
								callback(new Error('两次输入密码不一致!'));
							}else{
								callback();
							}
						}}
					],
					dept: [
						{required: true, message: '请选择所属部门'}
					],
					group: [
						{required: true, message: '请选择所属角色', trigger: 'change'}
					]
				},
				//所需数据选项
				groups: [],
				groupsProps: {
					value: "id",
					multiple: true,
					checkStrictly: true
				},
				depts: [],
				deptsProps: {
					value: "departmentId",
					emitPath: false,
					checkStrictly: true,
					children:"children"
				}
			}
		},
		mounted() {
			this.getGroup()
			this.getDept()
		},
		methods: {
			//显示
			open(mode='add'){
				this.mode = mode;
				this.visible = true;
				return this
			},
			//加载树数据
			async getGroup(){			
				var res = await this.$API.system.role.all.get();
				this.groups = res.data;
			},
			async getDept(){
				var res = await this.$API.system.dept.list.post();
				this.showGrouploading = true;
				this.treeData = this.convertToElTreeDeptData(res?.data);
				this.showGrouploading = false;
				this.depts = this.treeData;
			},
			//表单提交方法
			submit(){
				this.$refs.dialogForm.validate(async (valid) => {
					if (valid) {
						this.isSaveing = true;
						var res = null;
						if (this.mode === 'add') {
						    res = await this.$API.system.user.add.post(this.form);
						} else {
							res = await this.$API.system.user.update.put(this.form);
						}
						this.isSaveing = false;
						if(res.succeeded){
							this.$emit('success', this.form, this.mode)
							this.visible = false;
							this.$message.success("操作成功")
						}else{
							this.$alert(res.error, "提示", {type: 'error'})
						}
					}else{
						return false;
					}
				})
			},
			convertToElTreeDeptData(data, parentId = null) {
                const treeData = [];
                for (const item of data) {
                    if ((item.superiorId === parentId) || (parentId === null && !item.superiorId)) {
                        const children = this.convertToElTreeDeptData(data, item.id);
                        const treeNode = {
                            id: item.id,
							parentId: item.superiorId,
							sort:item.sort,
                            departmentName: item.departmentName,
							label: item.departmentName,
							isActive: item.isActive,
							created: item.created,
							description: item.description,
							departmentId:item.departmentId,
							concurrencyStamp:item.concurrencyStamp,
                            children: children.length > 0 ? children : null,
                        };
                        treeData.push(treeNode);
                    }
                }
                return treeData;
            },
			//表单注入数据
			setData(data){
				console.log("表单注入数据",data )
				this.form.id = data.id
				this.form.userId = data.userId
				this.form.userName = data.userName
				this.form.realname= data.realname
				this.form.roleIds= data.roleIds
				this.form.superiorId= data.superiorId
				this.form.departmentId= data.departmentId
				//this.form.departmentId = data.departmentId
				this.form.concurrencyStamp=data.concurrencyStamp
				this.form.profilePictureDataUrl= data.profilePictureDataUrl
			}
		}
	}
</script>

<style>
</style>
