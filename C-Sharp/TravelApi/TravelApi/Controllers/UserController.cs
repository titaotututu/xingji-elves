using TravelApi.Repository;
using TravelApi.Models;
using TravelApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cors;

namespace TravelApi.Controllers
{
    [EnableCors("AllowAll")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly Repository.TravelContext userDb;
        private readonly UserService userService;

        public UserController(Repository.TravelContext userDb)
        {
            this.userDb = userDb;
            userService = new UserService(userDb);
        }

        //�û���¼
        [HttpGet]
        public ActionResult<string> CheckLogin(long id, long pwd)
        // �Ѳ��ԣ��ɹ���
        // ʾ�� api��http://localhost:5199/api/User?id=1234&pwd=123456 ��������
        // �ɹ��Ļ�����id
        // ���ɹ�����text"�����ڸ��û����û��������"
        {
            var user = userService.CheckLogin(id, pwd);
            if (user == null)
            {

                return BadRequest("�����ڸ��û����û��������");
            }
            return Convert.ToString(user.UserId);
        }

        //�����û�
        [HttpPost]
        // �Ѳ��ԣ��ɹ���
        // ʾ����api http://localhost:5199/api/User 
        // �����壺json��ʽ��{ "UserId": 1234,"UserName": "wangyao","password": 123456 }
        // xml��ʽ�������У��ᱨ��
        // �ɹ��Ļ���������user
        // ���ɹ����ء����û��Ѵ��ڡ�
        public ActionResult<User> PostUser(User user)
        {
            try
            {
                if (userService.GetUser(user.UserId) == null)
                {
                    userService.AddUser(user);
                }
                else
                {
                    return BadRequest("���û��Ѿ����ڣ�");
                }
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            return user;

        }

        //ɾ��ָ��id�û�
        [HttpDelete("{id}")]
        // �Ѳ��ԣ��ɹ�
        // ʾ����api http://localhost:5199/api/User/1234 ��������
        // �ɹ��Ļ��޷���ֵ��
        // ���ɹ��Ļ�����text�������ڸ��û���
        public ActionResult DeleteUser(string id)
        {
            try
            {
                //ɾ��user�����user
                userService.RemoveUser(long.Parse(id));

            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            return NoContent();
        }

        //�޸�ָ��id�û��˺���Ϣ
        // �Ѳ��ԣ��ɹ�
        // ʾ����api http://localhost:5199/api/User/123
        // �����壺json��{"UserId": 123,"UserName": "duhuang2","password": 123456}
        // �޸ĳɹ��Ļ�û�з���ֵ�����ɹ��Ļ�����text"Id cannot be modified!"
        [HttpPut("{id}")]
        public ActionResult<User> ModifyUser(string id, User user)
        {
            if (long.Parse(id) != user.UserId)
            {
                return BadRequest("Id cannot be modified!");
            }
            try
            {
                userService.ModifyUser(user);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            return NoContent();
        }

        //�����û�id�����û�
        // �Ѳ��ԣ��ɹ�
        //ʾ���� api http://localhost:5199/api/User/123 ��������
        // �ɹ��Ļ�����user
        // ���ɹ��Ļ�û�з����ı���������Լ��ϣ�
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(string id)
        {
            User user;
            try
            {
                user = userService.GetUser(long.Parse(id));
                return user;
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }

        }

        //�����û����������û�
        // �Ѳ��ԣ��ɹ���
        // ʾ����api http://localhost:5199/api/User/name?name=duhuang ��������
        // ����ɹ���������user�����ʧ���򷵻�text"δ�ҵ�����Ҫ����û���"

        [HttpGet("name")]
        public ActionResult<List<User>> GetUserByName(string name)
        {

            try
            {
                var users = userService.QueryUserByName(name);
                if (users.Count > 0)
                {
                    return users;
                }
                else
                {
                    return BadRequest("δ�ҵ�����Ҫ����û���");
                }
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}