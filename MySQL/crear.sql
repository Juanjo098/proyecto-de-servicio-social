-- drop database titulacion;

create database titulacion;

use titulacion;

CREATE TABLE tipo_usuario (
	id_tipo_usuario INT AUTO_INCREMENT,
    nombre VARCHAR(16) NOT NULL UNIQUE,
    hab BIT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (id_tipo_usuario )
);

CREATE TABLE usuario (
	id_usuario CHAR(36) NOT NULL DEFAULT (UUID()),
    id_tipo_usuario INT NOT NULL DEFAULT 3,
    nombre VARCHAR(64) NOT NULL UNIQUE,
    correo VARCHAR(64) NOT NULL UNIQUE,
    contrasena CHAR(64) NOT NULL,
    mensajes_hab BIT(1) NOT NULL DEFAULT 0,
    hab BIT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (id_usuario),
    FOREIGN KEY (id_tipo_usuario) REFERENCES tipo_usuario(id_tipo_usuario )
);

create table departamento (
	id_dpto int auto_increment not null unique,
    id_jefe_dpto int,
    nombre varchar(128) not null unique,
    hab bit(1) not null default 1,
    primary key (id_dpto)
);

create table docente (
	id_docente int not null auto_increment,
    id_dpto int not null,
    nombre varchar(128) not null,
    titulo varchar(128) not null,
    diminutivo varchar(16) not null,
    cedula varchar(16) not null unique,
    hab bit(1) not null default 1,
    primary key (id_docente),
    foreign key (id_dpto) references departamento(id_dpto)
);

alter table departamento
add foreign key (id_jefe_dpto) references docente(id_docente);

create table carrera (
	id_carrera int auto_increment not null unique,
    id_dpto int not null,
    nombre varchar(64) not null,
    hab bit(1) not null default 1,
    primary key (id_carrera),
    foreign key (id_dpto) references departamento(id_dpto)
);

CREATE TABLE info_personal (
	no_control varchar(10) not null unique,
    id_usuario char(36) not null unique,
    id_carrera int not null,
    nombre varchar(64) not null,
    ap_paterno varchar(32) not null,
    ap_materno varchar(32) not null,
    telefono varchar(16) not null,
    direccion varchar(128),
    hab bit(1) not null default 1,
    primary key (no_control),
    foreign key (id_usuario) references usuario(id_usuario),
    foreign key (id_carrera) references carrera(id_carrera)
);

create table cargo (
	id_cargo int auto_increment not null,
    nombre varchar(128) not null unique,
    hab bit(1) not null default 1,
    primary key (id_cargo)
);

create table docente_cargo (
	id_docente int not null,
    id_cargo int not null,
    foreign key (id_docente) references docente(id_docente),
    foreign key (id_cargo) references cargo(id_cargo)
);

create table opciones
(
	id_opcion int auto_increment not null,
	opcion varchar(60) not null,
	hab bit(1) not null default 1,
    primary key (id_opcion)
);

create table producto
(
	id_producto int auto_increment not null,
	producto varchar(60) not null,
	hab bit(1) not null default 1,
    primary key (id_producto)
);

create table alternativa
(
	id_alternativa int auto_increment not null,
	alternativa varchar(60) not null,
	hab bit(1) not null default 1,
    primary key (id_alternativa)
);

-- drop table proceso_titulacion;
create table proceso_titulacion (
	id_proceso int auto_increment not null,
    no_control varchar(10) not null unique,
	paso1 bit(2) not null default 3,
    scni bit(2) not null default 0,
    cni bit(2) not null default 0,
    cl bit(2) not null default 0,
    caii bit(2) not null default 0,
    rp bit(2) not null default 0,
    rps bit(2) not null default 0,
    st bit(2) not null default 0,
    pro bit(2) not null default 0,
    sl bit(2) not null default 0,
    lp bit(2) not null default 0,
    asnc bit(2) not null default 0,
    oi bit(2) not null default 0,
    curp bit(2) not null default 0,
    rfc bit(2) not null default 0,
    cb bit(2) not null default 0,
    hab bit(1) not null default 1,
    primary key (id_proceso),
    foreign key (no_control) references info_personal(no_control)
);

-- drop table informacion_titulacion;
create table informacion_titulacion(
	id_info_titulacion int auto_increment not null,
    no_control varchar(10) not null unique,
	fecha_cni date,
    fecha_st date,
    fecha_aarp date,
    fecha_arp date,
    hora_arp time,
    fecha_vecimiento date,
    producto varchar(60),
    alternativa varchar(60),
    proyecto varchar (256),
    presidente int,
    secretario int,
    vocal int,
    suplente int,
    hab bit(1) not null default 1,
    estado bit(2) not null default 0,
    primary key (id_info_titulacion),
    foreign key (no_control) references info_personal(no_control)
);
