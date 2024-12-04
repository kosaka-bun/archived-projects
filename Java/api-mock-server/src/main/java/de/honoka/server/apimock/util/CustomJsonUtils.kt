package de.honoka.server.apimock.util

import de.marhali.json5.Json5
import de.marhali.json5.Json5Options

object CustomJsonUtils {

    private val json5Utils = Json5(Json5Options(
        false, false, false, 4
    ))

    fun json5ToJson(json5: String): String = json5Utils.serialize(json5Utils.parse(json5))
}