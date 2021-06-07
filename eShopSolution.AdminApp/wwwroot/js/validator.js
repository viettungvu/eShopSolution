function Validator(object){
    var formElement=document.querySelector(object.name);
    function validate(inputElement, rule){
        var message=rule.test(inputElement.value); 
        if(message){
            inputElement.parentNode.classList.remove("success");
            inputElement.parentNode.classList.add("danger");
            inputElement.parentNode.querySelector('.error-message').innerText=message;
        }
        else{
            inputElement.parentNode.classList.remove("danger");
            inputElement.parentNode.classList.add("success");
        }
    }   
    if(formElement){
        object.rules.forEach(function(rule){
           var inputElement=formElement.querySelector(rule.selector);
           if(inputElement){
               inputElement.onblur=function(){  
                   validate(inputElement, rule);           
               }
               inputElement.oninput=function(){
                   inputElement.parentNode.classList.remove("danger");
                   inputElement.parentNode.classList.remove("success");
               }
           }
        });
    }
}


Validator.isRequired=function(selector){
    return {
        selector:selector,
        test:function(value){
            return value.trim().slice(1, value.length)? undefined:selector.toUpperCase()+ ' không được bỏ trống';
        }
    }
}
Validator.isEmail=function(selector){
    return {
        selector:selector,
        test:function(email){ 
            var regex=/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
            return regex.test(email)?undefined:'Email không hợp lệ';
        }

    }
}
Validator.isPhone=function(selector){
    return{
        selector:selector,
        test:function(phone){
            var regex=/([84|0]+[1|3|5|7|8|9])+([0-9]{8})\b/
            return regex.test(phone)?undefined:'Phone không hợp lệ';
        }
    }
}
Validator.minLength=function(selector, minLength){
    return{
        selector:selector,
        test:function(password){
            return password.length>=minLength?undefined:'Mật khẩu phải dài hơn '+minLength+' kí tự';
        }
    }
}
