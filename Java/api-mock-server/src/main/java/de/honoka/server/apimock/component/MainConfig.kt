package de.honoka.server.apimock.component

import de.honoka.sdk.util.code.ColorfulText
import org.slf4j.Logger
import org.slf4j.LoggerFactory
import org.springframework.boot.context.properties.ConfigurationProperties
import org.springframework.boot.context.properties.EnableConfigurationProperties
import org.springframework.context.annotation.Configuration
import javax.annotation.PostConstruct

@ConfigurationProperties("app")
data class MainProperties(

    var responseFileBase: String? = "./response_files"
)

@EnableConfigurationProperties(MainProperties::class)
@Configuration
class MainConfig(
    private val mainProperties: MainProperties
) {

    private val log: Logger = LoggerFactory.getLogger(MainConfig::class.java)

    @PostConstruct
    fun init() {
        log.info(ColorfulText.of().apply {
            green("Use responseFileBase: ${mainProperties.responseFileBase}")
        }.toString())
    }
}