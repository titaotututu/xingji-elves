using TravelApi.Repository;
using TravelApi.Models;

namespace TravelApi.Services
{
	public class UserService
	{

		TravelContext dbContext;
		public UserService(TravelContext dbContext)
		{
			this.dbContext = dbContext;
		}


		public void AddUser(User user)
		{

			dbContext.Users.Add(user);
			dbContext.SaveChanges();

		}


		public void RemoveUser(long userId)
		{
			var user = dbContext.Users.FirstOrDefault(x => x.UserId == userId);
			if (user != null)
			{
				dbContext.Users.Remove(user);
				dbContext.SaveChanges();
			}
			else
			{
				throw new ArgumentException("删除用户失败！");
			}

		}


		public User GetUser(long userId)
		{
			return dbContext.Users.FirstOrDefault(x => x.UserId == userId);
		}


		public List<User> QueryUserByName(string username)
		{
			var query = dbContext.Users
				.Where(user => user.UserName == username);
			return query.ToList();
		}


		public void ModifyUser(User newuser)
		{
			RemoveUser(newuser.UserId);
			AddUser(newuser);
		}


		public User CheckLogin(long id, long pwd)
		{
			var user = dbContext.Users.FirstOrDefault(x => x.UserId == id
													&& x.password == pwd);
			return user;

		}




	}
}