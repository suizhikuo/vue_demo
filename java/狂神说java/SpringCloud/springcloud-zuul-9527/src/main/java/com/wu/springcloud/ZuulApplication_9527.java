package com.wu.springcloud;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cloud.netflix.zuul.EnableZuulProxy;

@SpringBootApplication
@EnableZuulProxy
public class ZuulApplication_9527 {
    public static void main(String[] args) {
        SpringApplication.run(ZuulApplication_9527.class, args);
    }

    //http://www.wu.com:9527/SPRINGCLOUD-PROVIDER-DEPT/dept/get/1
    //http://www.wu.com:9527/springcloud-provider-dept/dept/get/1
}
