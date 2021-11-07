package com.wu.springcloud.controller;


import com.wu.springcloud.pojo.Dept;
import com.wu.springcloud.service.DeptClientService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseBody;

import java.util.List;

@Controller
public class DeptConsumerController {

    @Autowired
    private DeptClientService deptClientService = null;

    @RequestMapping("/consumer/dept/get/{id}")
    @ResponseBody
    public Dept queryById(@PathVariable("id") Long id) {
        return this.deptClientService.queryById(id);
    }

    @RequestMapping("/consumer/dept/add")
    @ResponseBody
    public boolean add(Dept dept) {
        return this.deptClientService.addDept(dept);
    }

    @RequestMapping("/consumer/dept/list")
    @ResponseBody
    public List<Dept> queryAll() {
        return this.deptClientService.queryAll();
    }


}
