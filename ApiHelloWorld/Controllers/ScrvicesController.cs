using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiHelloWorld.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScrvicesController : ControllerBase
    {
        //api 호출 URI 구성: [controller] 대신에 클레스 이름에서  controller 앞부분만 붙여서 호출
        //api/Services
        //api/Services 기본적으로 Get() 메서드가 호출

        // GET: api/<ScrvicesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //api/Services/1234
        // GET api/<ScrvicesController>/5
        //[HttpGet("{id=int}")]   //타입 제약
        //[HttpGet("{id=1234}")]   //기본값
        /*public string Get([FromRoute] int id, [FromQuery] string query)
        {
            return $"넘어온 값: {id}, {query}";
        }
*/
        [HttpGet("{id=int}")]
        public IActionResult Get(int id)
        {
            return Ok(new Dto { Id = id, Text = $"값: {id}"});
        }
        //api/Services
        // POST api/<ScrvicesController>
        [HttpPost]
        public IActionResult Post([FromBody] Dto value)
        {
            //모델 유효성 검사
            if(!ModelState.IsValid) //유효하지 않으면
            {
                //에러 발생
                //throw new InvalidOperationException("잘못된 데이터 입니다");
                return BadRequest(ModelState);      //400
            }

            //넘어온 데이터를 저장, 연산, 가공... ...
            //return Ok();        //200

            return CreatedAtAction("Get", new {id = value.Id}, value);
        }


        //api/Serivices/1234
        // PUT api/<ScrvicesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {

        }

        //api/Serivices/5678
        // DELETE api/<ScrvicesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
    //모델 클레스
    public class Dto
    {
        public int Id { get; set; }
        [MinLength(5)]
        public string Text { get; set; }
    }
}

/*

특성(Attribute, 어트리뷰트)
[HttpGet]
[HttpGet("{id=int}")]
[HttpPost]
[HttpPut("{id}")]
[HttpDelete("{id}")]

//api/Services 호출시 특성을 붙이면 함수이름과 상관 없다
GET 방식으로 호출하면 [HttpGet] 붙은 함수가 응답
GET 방식으로 호출하면서 매개변수가 붙은 면 [HttpGet("{id=1234}")]붙은 함수가 응답
POST/PUT/Delete 방식으로 호출하면 [HttpPost]/[HttpPut("{id}")]/[HttpDelete("{id}")] 붙은함수가 응답

[FromRoute] :default
[FromBody]
[FromQuery]
/1234 : 기본 라우트값

-- 경로 설정 예제 : api/Services/1234?id=5678
: 라우트 토큰 : "api/[controller]"
: 기본 라우트 값 [FromRoute] : /1234
: 선택적 라우트 값 
[FromQuery] : ?id=5678

-- IActionResult 반환시 응답 값
OK              --상태코드(값) 200
BadRequest      --상태코드(값) 400
CreateAtAction  --상태코드(값) 201

 */
