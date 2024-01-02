using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using TaskManagerTut.DTO;
using TaskManagerTut.Entity;
using TaskEntity = TaskManagerTut.Entity.Task1; // Alias for clarity


namespace TaskManagerTut.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        public string URI = "https://localhost:8081";
        public string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public string DatabaseName = "TestDB";
        public string ContainerName = "TestContainer3";

        public Container container; // null 
        public TaskController()
        {
            container = GetContainer();
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(TaskModel taskModel)
        {
            TaskEntity taskEntity = new TaskEntity(); // Use the alias
                                                      // Mapping 
            taskEntity.Title = taskModel.Title;
            taskEntity.Description = taskModel.Description;
            taskEntity.TaskNo = taskModel.TaskNo;

            // mandatory fields 
            taskEntity.Id = Guid.NewGuid().ToString(); // 16-digit hex code
            taskEntity.UId = taskEntity.Id;
            taskEntity.DocumentType = "task";

            taskEntity.CreatedOn = DateTime.Now;
            taskEntity.CreatedByName = "Ritesh";
            taskEntity.CreatedBy = "Ritesh's UId";

            taskEntity.UpdatedOn = DateTime.Now;
            taskEntity.UpdatedByName = "Ritesh";
            taskEntity.UpdatedBy = "Ritesh's UId";

            taskEntity.Version = 1;
            taskEntity.Active = true;
            taskEntity.Archived = false;  // Not Accessible to System

            //step 3: Add data to database
            taskEntity = await container.CreateItemAsync(taskEntity);

            //Step 4: Return model to ui
            TaskModel model = new TaskModel();
            model.UId = taskEntity.UId;
            model.TaskNo = taskEntity.TaskNo;
            model.Title = taskEntity.Title;
            model.Description = taskEntity.Description;

            return Ok(model);
        }
        //Entity------->database
        //DTO-------->UI

        [HttpPost]
        public async Task<IActionResult> GetAllTasks()
        {
            //Get All Students
            var tasks = container.GetItemLinqQueryable<Task1>(true).Where(q => q.DocumentType == "task" && q.Archived == false && q.Active == true).
                AsEnumerable().ToList();

            //Step:2 Map all student Data
            List<TaskModel> taskModelList = new List<TaskModel>();
            foreach (var task in tasks)
            {
                TaskModel model = new TaskModel();
                model.UId = task.UId;
                model.TaskNo = task.TaskNo;
                model.Title = task.Title;
                model.Description = task.Description;

                taskModelList.Add(model);
            }

            return Ok(taskModelList);
        }

        [HttpPost]
        public async Task<IActionResult> GetStudentByUID(string TaskUId)
        {
            //Get Student by UId
            var tasks = container.GetItemLinqQueryable<Task1>(true).Where(q => q.UId == TaskUId && q.DocumentType == "task" && q.Archived == false && q.Active == true).
                AsEnumerable().FirstOrDefault();

            //Convert entity class to model
            TaskModel model = new TaskModel();
            model.UId = tasks.UId;
            model.TaskNo = tasks.TaskNo;
            model.Title = tasks.Title;
            model.Description = tasks.Description;

            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStudent(TaskModel taskModel)
        {
            var existingStudent = container.GetItemLinqQueryable<Task1>(true).Where(q => q.UId == taskModel.UId && q.DocumentType == "task" && q.Archived == false && q.Active == true).
                AsEnumerable().FirstOrDefault();
            existingStudent.Archived = true;
            await container.ReplaceItemAsync(existingStudent, existingStudent.Id);

            //Assign Mandatory Fields
            existingStudent.Id = Guid.NewGuid().ToString();
            existingStudent.UpdatedBy = "Ritesh";
            existingStudent.UpdatedByName = "Ritesh";
            existingStudent.Version = existingStudent.Version + 1;
            existingStudent.Active = true;
            existingStudent.Archived = false;

            //Assign UI model Fields
            existingStudent.TaskNo = taskModel.TaskNo;
            existingStudent.Title = taskModel.Title;
            existingStudent.Description = taskModel.Description;

            //Add Data to database
            existingStudent = await container.CreateItemAsync(existingStudent);

            //Return Model
            TaskModel model = new TaskModel();
            model.UId = existingStudent.UId;
            model.TaskNo = existingStudent.TaskNo;
            model.Title = existingStudent.Title;
            model.Description = existingStudent.Description;


            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudent(string TaskUId)
        {
            //Get existing data
            var tasks = container.GetItemLinqQueryable<Task1>(true).Where(q => q.UId == TaskUId && q.DocumentType == "task" && q.Archived == false && q.Active == true).
                AsEnumerable().FirstOrDefault();
            tasks.Active = false;
            await container.ReplaceItemAsync(tasks, tasks.Id);

            return Ok(true);
        }



        private Container GetContainer() // DRY
        {
            CosmosClient cosmosclient = new CosmosClient(URI, PrimaryKey);
            // step 2 Connect with Our Database
            Database databse = cosmosclient.GetDatabase(DatabaseName);
            // step 3 Connect with Our Container 
            Container container = databse.GetContainer(ContainerName);

            return container;
        }
    }
}
