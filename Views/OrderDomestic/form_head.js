﻿function form_head_ini() {
    var store_BUSITYPE = Ext.create('Ext.data.JsonStore', {//进出口类型
        fields: ['CODE', 'NAME'],
        data: ietype_js_data
    })
    var combo_BUSITYPE = Ext.create('Ext.form.field.ComboBox', {
        id: 'IETYPE1',
        name: 'IETYPE',
        store: store_BUSITYPE,
        hideTrigger: true,
        fieldLabel: '进出口类型',
        displayField: 'NAME',
        valueField: 'CODE',
        tabIndex: 2,
        queryMode: 'local',
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.expand();
                }
            }, 
            change: function (cb, newValue, oldValue, eOpts) {
                if (newValue == "仅进口") {
                    formpanelin.setVisible(true);
                    formpanelout.setVisible(false);
                }
                if (newValue == "仅出口") {
                    formpanelin.setVisible(false);
                    formpanelout.setVisible(true);
                }
                if (newValue == "进/出口业务") {
                    formpanelin.setVisible(true);
                    formpanelout.setVisible(true);
                }
            }
        },
        labelWidth: 80,
        allowBlank: false,
        value: '进/出口业务',
        blankText: '进出口类型不能为空!'
    })
    //申报方式
    var store_REPWAYNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbfs
    })
    var combo_REPWAYNAME = Ext.create('Ext.form.field.ComboBox', {
        name: 'REPWAYID',
        store: store_REPWAYNAME,
        hideTrigger: true,
        fieldLabel: '申报方式',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 3,
        queryMode: 'local',
        anyMatch: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.expand();
                }
            }
        },
        labelWidth: 80,
        allowBlank: false,
        blankText: '申报方式不能为空!'
    })
    //申报关区
    var store_CUSTOMDISTRICTNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbgq
    })
    var combo_CUSTOMDISTRICTNAME = Ext.create('Ext.form.field.ComboBox', {//申报关区 这个数据比较多需要根据输入字符到后台动态模糊匹配
        name: 'CUSTOMDISTRICTCODE',
        store: store_CUSTOMDISTRICTNAME,
        fieldLabel: '申报关区',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        hideTrigger: true,
        forceSelection: true,
        tabIndex: 4,
        anyMatch: true,
        allowBlank: false,
        minChars: 2,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                }
            },
            select: function (cb, records, eOpts) {
                field_CUSTOMDISTRICTNAME.setValue(cb.getRawValue().substr(0, cb.getRawValue().lastIndexOf('(')));
            }
        },
        blankText: '申报关区不能为空!',
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    var field_CUSTOMDISTRICTNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'CUSTOMDISTRICTNAME'
    })
    var field_ordercodes = Ext.create('Ext.form.field.Hidden', {
        id: 'field_ordercodes',
        //存放表单加载完毕后,所以已经存在的订单号，用逗号分隔,当改变进出口类型时，需要将原始的和新的订单号进行比对，并删除新集合中不存在的订单及其附属信息
        name: 'ORDERCODES'
    })
    var file_addition = Ext.create('Ext.form.field.Hidden', {
        id: 'file_addition',
        //该隐藏字段不属于订单，主要用户提交后记录当前订单是否开启了上传文件的开关
        name: 'ADDITION'
    })
    //委托时间
    var field_SUBMITTIME = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITTIME',
        fieldLabel: '委托时间',
        readOnly: true
    });
    //委托人员
    var field_SUBMITUSERNAME = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITUSERNAME',
        fieldLabel: '委托人员',
        readOnly: true
    });
    //维护人员
    var field_CREATEUSERNAME = Ext.create('Ext.form.field.Text', {
        name: 'CREATEUSERNAME',
        fieldLabel: '维护人员',
        readOnly: true
    });
    //维护时间
    var field_CREATETIME = Ext.create('Ext.form.field.Text', {
        name: 'CREATETIME',
        fieldLabel: '维护时间',
        readOnly: true
    });
    //平台客服
    var field_CSNAME = Ext.create('Ext.form.field.Text', {
        name: 'CSNAME',
        fieldLabel: '平台客服',
        readOnly: true
    });
    //客服电话
    var field_CSPHONE = Ext.create('Ext.form.field.Text', {
        name: 'CSPHONE',
        fieldLabel: '客服电话',
        readOnly: true
    });
    //委托电话
    var field_SUBMITUSERPHONE = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITUSERPHONE',
        fieldLabel: '委托电话',
        readOnly: true
    })
    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;基础信息</span></h4>'
    }
    topbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo]
    })
    formpanelhead = Ext.create('Ext.form.Panel', {
        border: 0,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: .20,
            labelSeparator: '',
            msgTarget: 'under',
            labelAlign: 'right',
            validateOnBlur: false,
            validateOnChange: false
        },
        items: [
                {
                    layout: 'column', height: 42, border: 0, margin: '5 0 0 0', items: [
                      combo_BUSITYPE, combo_REPWAYNAME, combo_CUSTOMDISTRICTNAME, field_SUBMITTIME, field_SUBMITUSERNAME
                    ]
                },
                {
                    layout: 'column', height: 42, border: 0, items: [
                      field_CREATEUSERNAME, field_CREATETIME, field_CSNAME, field_CSPHONE, field_SUBMITUSERPHONE
                    ]
                },
        field_CUSTOMDISTRICTNAME, field_ordercodes, file_addition
        ]
    })
}