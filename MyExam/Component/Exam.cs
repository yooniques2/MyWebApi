using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace MyExam.Component
{
    public class Exam
    {
        //Empty

    }

    /// <summary>
    /// 리포지토리 인터페이스 - 기능정의
    /// </summary>
    public interface IQuestionRepository
    {
        Question Add(Question question);
        List<Question> GetAll();
        Question GetById(int id);
        Question Update(Question question);
        void Delete(int id);
    }

    /// <summary>
    /// 리포지토리 클래스 - 기능 구현
    /// </summary>
    public class QuestionRepository : IQuestionRepository
    {
        private IConfiguration _config;
        private IDbConnection db;

        //생성자
        public QuestionRepository(IConfiguration config)
        {
            _config = config;
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings")
                .GetSection("DefaultConnection").Value);

        }
        public Question Add(Question question)
        {
            string sql = @"
                insert into Question (Title) values (@Title)
                select cast(SCOPE_IDENTITY() as int);
                ";
            var id = db.Query<int>(sql, question).Single();
            question.Id = id;
            return question;
        }

        public List<Question> GetAll()
        {
            string sql = @"
                select * from Question order by Id desc;
                ";
            return db.Query<Question>(sql).ToList();
        }

        public Question GetById(int id)
        {
            /*
            //sql명령문
            string sql = @"
                select * from Question where Id = @Id;
                ";
            return db.Query<Question>(sql, new {Id = id}).Single();
        */

            string sql = "usp_getbyId@Id";
            return db.Query<Question>(sql, new {Id = id }).Single();
        }

        public Question Update(Question question)
        {
            string sql = "update Question" +
                "set" +
                "TItle = @Title "+
                "where Id = @Id";
            db.Execute(sql, question);
            return question;
            
        }
        public void Delete(int id)
        {
            string sql = @"
                delete from Question where Id = @Id;
                ";
            db.Execute(sql, new {Id = id});
        }
    }

    /// <summary>
    /// 웹 api 컨트롤러 클래스
    /// </summary>
    [Route("api/[controller]")]
    public class QuestionServicecontroller : ControllerBase
    {
        private IQuestionRepository _repository;

        //생성자
        public QuestionServicecontroller(IQuestionRepository repository)
        {
            _repository = repository;
        }


        [HttpGet]
        public IActionResult Get()
        {
            try
            {
               var questions = _repository.GetAll();
                if (questions == null)
                {
                    return NotFound("데이터가 없습니다");
                }
                return Ok(questions);
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                var model = _repository.GetById(id);
                if (model == null)
                {
                    return NotFound($"{id}번 데이터가 없습니다");

                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest($"에러가 발생하였습니다.{ex.Message}");
            }
        }
        [HttpPost]
        public IActionResult Post([FromBody] Question model)
        {
            //예외처리
            if (model == null)
            {
                return BadRequest();
            }

            try
            {
                //예외처리
                if (model.Title == null || model.Title.Length < 1)
                {
                    ModelState.AddModelError("Title", "문제를 입력해야 합니다");
                }
                //모델유효성 검사
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                //매개변수로 들어온 데이터를 리포지토리에 넘겨준다
                var newModel = new Question() { Id = model.Id, Title = model.Title};
                var m = _repository.Add(newModel);

                return Ok(m);

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] Question question)
        {
            //예외처리
            if (question == null)
            {
                return BadRequest();
            }
            try
            {
                //데이터 존재 여부 체크
                var oldModel = _repository.GetById(id);
                if (oldModel == null)
                {
                    return NotFound($"{id}번 문제 데이터가 없습니다");
                }
                question.Id = id;
                var model = _repository.Update(question);
                return Ok(model);

            }
            catch (Exception ex)
            {
                return BadRequest($"에러가 발생하였습니다.{ex.Message}");
            }
        }



        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                //체크 - 데이터 존재여부
                var oldModel = _repository.GetById(id);
                if (oldModel == null)
                {
                    return NotFound($"{id}번 문제 데이터가 없습니다");

                }

                _repository.Delete(id);
                return NoContent();

            }
            catch (Exception ex)
            {
                return BadRequest($"에러가 발생하였습니다.{ex.Message}");
            }
        }
    }

    /// <summary>
    /// 모델클래스  - DB 테이블과 일대일 매핑
    /// </summary>
    public class Question
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(4000, ErrorMessage = "문제는 4000자 이하로 입력하세요")]
        public string Title { get; set; }
    }
}
