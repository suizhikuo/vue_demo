package com.wu.myrule;

import com.netflix.loadbalancer.IRule;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class WuRule {
    //IRule
    //RoundRobinRule:轮询
    //RandomRule：随机
    //AvailabilityFilteringRule：会先过滤掉跳闸、访问故障的服务~，对剩下的进行轮询
    //RetryRule：会先按照轮询获取服务，如果服务获取失败，则会在指定的时间内进行重试

    @Bean
    public IRule myRule() {
        return new WuRandomRule(); //默认为轮询，现在试使用自己自定义的
    }
}
