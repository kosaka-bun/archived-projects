package de.honoka.server.apimock

import de.honoka.sdk.util.framework.spring.gui.SpringBootConsoleWindow
import org.springframework.boot.autoconfigure.SpringBootApplication

@SpringBootApplication
class ApiMockServerApplication

fun main(args: Array<String>) {
    SpringBootConsoleWindow.of(
        "API Mock Server",
        1.25,
        ApiMockServerApplication::class.java
    ).run {
        configureWindowBuilder {
            it.isBackgroundMode = true
            it.isShowOnBuild = false
        }
        applicationArgs = args
        createAndRun()
    }
}