package de.honoka.server.apimock.component

import cn.hutool.core.exceptions.ExceptionUtil
import de.honoka.sdk.util.framework.web.ApiResponse
import org.springframework.http.HttpStatus
import org.springframework.web.bind.annotation.ExceptionHandler
import org.springframework.web.bind.annotation.RestControllerAdvice
import java.io.FileNotFoundException
import javax.servlet.http.HttpServletResponse

@RestControllerAdvice
class GlobalExceptionHandler {

    @ExceptionHandler(Throwable::class)
    fun handle(t: Throwable, response: HttpServletResponse): ApiResponse<*> {
        response.status = HttpStatus.INTERNAL_SERVER_ERROR.value()
        return ApiResponse.fail(ExceptionUtil.getMessage(t))
    }

    @ExceptionHandler(FileNotFoundException::class)
    fun handle(fnfe: FileNotFoundException, response: HttpServletResponse): ApiResponse<*> {
        response.status = HttpStatus.NOT_FOUND.value()
        return ApiResponse.fail(ExceptionUtil.getMessage(fnfe)).setCode(HttpStatus.NOT_FOUND.value())
    }
}