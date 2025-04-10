<template>
  <div class="app-container">
    <div class="card">
      <h1>Juego de ruleta</h1>

      <div v-if="!juegoIniciado" class="form-group">
        <label for="nombre">Nombre del jugador</label>
        <input v-model="nombre" placeholder="Tu nombre..." />

        <label for="saldoInicial">Saldo inicial:</label>
        <input type="number" v-model.number="saldoInicial" placeholder="Saldo inicial" />

        <label for="apuesta">Monto apuesta:</label>
        <input type="number" v-model.number="apuesta" placeholder="Apuesta" />

        <div class="button-group">
          <button @click="iniciarJuego">Iniciar juego</button>
          <button @click="cargarSaldo">Cargar saldo</button>
        </div>
      </div>

      <div v-else class="juego">
        <label><strong>Jugador:</strong> {{ nombre }}</label>
        <label><strong>Saldo actual:</strong> ${{ saldo.toFixed(2)}}</label>
        <label><strong>Monto de apuesta:</strong> ${{ apuesta.toFixed(2)}}</label>

        <label for="tipoApuesta">Tipo de apuesta apuesta:</label>
        <select v-model="tipoApuesta">
          <option value="1">Color (rojo/negro)</option>
          <option value="2">Par/impar de color</option>
          <option value="3">Numero y color</option>
        </select>

        <input v-if="Number(tipoApuesta) === 1" v-model="colorApuesta" placeholder="Rojo o Negro" />

        <div v-if="Number(tipoApuesta) === 2">
          <input v-model="colorApuesta" placeholder="Rojo o negro" />
          <input v-model="parApuesta" placeholder="Par o impar" />
        </div>

        <div v-if="Number(tipoApuesta) === 3">
          <input v-model.number="numeroApuesta" placeholder="Número (0-36)" />
          <input v-model="colorApuesta" placeholder="Rojo o negro" />
        </div>

        <div class="button-group">
          <button @click="apostar">Apostar</button>
          <button @click="guardarPartida">Guardar partida</button>
        </div>
      </div>

      <div v-if="resultado" class="resultado">
        <h3>Resultado de la ruleta:</h3>
        <label><strong>Numero:</strong> {{ resultado.numero }}</label>
        <label><strong>Color:</strong> {{ resultado.color }}</label>
        <label><strong>{{ resultado.mensaje }}</strong></label>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue'
  import axios from 'axios'

  //const URL_BASE_API = "https://localhost:7243/api"
  const URL_BASE_API = "https://localhost/PruebaUnilink_API/api"
  const nombre = ref('')
  const saldoInicial = ref(0)
  const saldo = ref(0)
  const apuesta = ref(0)
  const tipoApuesta = ref(1)
  const colorApuesta = ref(null)
  const parApuesta = ref(null)
  const numeroApuesta = ref(null)
  const juegoIniciado = ref(false)
  const resultado = ref(null)
  const esSaldoCargado = ref(false)
  const esNuevoJugador = ref(false)

  const iniciarJuego = () => {
    if (!nombre.value.trim() || saldoInicial.value <= 0 || apuesta.value <= 0) {
      alert("Para poder iniciar un juego debes especificar nombre, saldo y un monto de apuesta.");
      return;
    }
    else if (apuesta.value > saldoInicial.value) {
      alert("El monto de la apuesta no puede ser mayor al saldo indicado.");
      return;
    }

    axios.get(`${URL_BASE_API}/Usuario/Obtener/${nombre.value.toLowerCase()}`, {
    })
      .then(res => {
        if (res.data === true) { // El nombre de usuario ya existe
          if (!esNuevoJugador.value && !esSaldoCargado.value) {
            alert("El nombre de usuario ya existe, por favor elige otro.");
            return; // No permitir iniciar el juego
          } else {
            // Si es un jugador existente (no nuevo o con saldo cargado), permitir iniciar el juego
            saldo.value = saldoInicial.value;
            saldoInicial.value = saldo.value;
            juegoIniciado.value = true;
          }
        } else { // El nombre de usuario no existe (está disponible)
          saldo.value = saldoInicial.value;
          saldoInicial.value = saldo.value;
          juegoIniciado.value = true;
        }
      })
      .catch(error => {
        alert(`Ha ocurrido un problema inesperado: ${error.message}`);
      });
  }

  const cargarSaldo = async () => {
    if (!nombre.value.trim()) {
      alert("Para poder cargar saldo debes especificar un nombre y un monto de apuesta valido.");
      return;
    }

    axios.get(`${URL_BASE_API}/Usuario/CargarSaldo/${nombre.value.toLowerCase()}`, {
      validateStatus: function (status) {
        return (status >= 200 && status < 300) || status === 404;
      }
    })
      .then(res => {
        if (res.status === 404) {
          alert(res.data);
          return;
        } else if (res.status >= 200 && res.status < 300) {
          saldoInicial.value = res.data.saldo;
          saldo.value = saldo.value || saldoInicial.value;
          esSaldoCargado.value = true;
          return;
        } else {
          alert(`Ocurrio un error al cargar el saldo (codigo: ${res.status})`);
        }
      })
      .catch(error => {
        alert(`Ha ocurrido un problema inesperado: ${error.message}`);
      });
  }

  const apostar = async () => {
    const apuestaData = {
      nombre: nombre.value,
      tiposApuesta: Number(tipoApuesta.value),
      color: colorApuesta.value,
      paridad: parApuesta.value,
      numero: Number(numeroApuesta.value),
      monto: parseFloat(apuesta.value),
      saldo: parseFloat(saldo.value)
    }

    if (parseFloat(apuesta.value) > parseFloat(saldo.value)) {
      alert("Lo sentimos, no cuentas con suficiente saldo para la apuesta.");
      return;
    }

    if (Number(tipoApuesta.value) === 1 && !(colorApuesta.value.toLowerCase() === "rojo" ||
      colorApuesta.value.toLowerCase() === "negro")) {
      alert("Debes indicar un color válido para la apuesta (rojo o negro).");
      return;
    }
    else if (Number(tipoApuesta.value) === 2 &&
      (!(colorApuesta.value.toLowerCase() === "rojo" || colorApuesta.value.toLowerCase() === "negro") ||
        !(parApuesta.value.toLowerCase() === "par" || parApuesta.value.toLowerCase() === "impar"))) {
      alert("Para la apuesta par/impar de color, debes indicar un color (rojo o negro) y si es par o impar.");
      return;
    }
    else if (Number(tipoApuesta.value) === 3 &&
      !(colorApuesta.value.toLowerCase() === "rojo" || colorApuesta.value.toLowerCase() === "negro") ||
      (isNaN(Number(numeroApuesta.value)) ||
        !Number.isInteger(Number(numeroApuesta.value)) ||
        Number(numeroApuesta.value) < 0 ||
        Number(numeroApuesta.value) > 36)) {
      alert("Para la apuesta numero y color, debes indicar un color (rojo o negro) y un número entre 0 y 36.");
      return;
    }

    try {
      const res = await axios.post(`${URL_BASE_API}/Ruleta/Apostar`, apuestaData);

      if (res.status === 200) {
        resultado.value = res.data
        saldo.value = res.data.nuevoSaldo
      }
      else {
        alert("Ocurrio un error al obtener los resultados de la apuesta.");
        return;
      }

    }
    catch (error) {
      alert(`Ha ocurrido un problema inesperado: ${error.message}`);
    }
  }

  const guardarPartida = async () => {
    const usuarioData = {
      nombre: nombre.value.toLowerCase(),
      saldo: parseFloat(saldo.value)
    }

    if (!nombre.value || parseFloat(saldo.value) < 0) {
      alert("Se debe indicar un nombre y un saldo mayor o igual a 0 para guardar la partida en curso.");
      return;
    }

    try {
      const res = await axios.post(`${URL_BASE_API}/Usuario/GuardarDatos`, usuarioData);

      switch (res.status) {
        case 200:
          saldo.value = res.data.saldo
          alert("Datos guardados correctamente.");
          break;
        case 404:
          alert(res.data);
          break;
        default:
          alert(`Ocurrio un error al guardar los datos (codigo: ${res.status})`);
      }
    }
    catch (error) {
      alert(`Ha ocurrido un problema inesperado: ${error.message}`);
    }
  }
</script>
